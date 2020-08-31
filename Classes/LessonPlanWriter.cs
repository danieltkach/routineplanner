using SelectPdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Routine_Planner
{
    public static class LessonPlanWriter
    {
        public struct LessonPlanOptions
        {
            public Difficulty difficulty;
            public int rounds;
            public bool includeCoolDown;
            public bool includeZejax;
            public SplitKind selectedSplit;
            public bool openBrowser;
            public bool createPDF;
            public string routineName;
        }
        public static LessonPlanOptions lessonPlanOptions = new LessonPlanOptions();
        
        //public static void SaveLessonPlan(LessonPlanRoutine lessonPlan, string routineName, bool openBrowser, bool createPDF)
        public static void SaveLessonPlan(LessonPlanRoutine lessonPlan)
        {
            SaveHTMLFile(lessonPlan);
            SavePDF();
            StoreIntoMyRoutinesList();
            SaveRoutinePlannerFileFormat(lessonPlan);
        }

        public static void SaveRoutinePlannerFileFormat(LessonPlanRoutine lessonPlan)
        {
            string fileName = lessonPlanOptions.routineName + GlobalVar.ProgramExtension;
            StreamWriter stream = new StreamWriter(fileName);
            const int categories = 5;
            List<CourseExercise>[] exerciseLists = new List<CourseExercise>[categories];
            exerciseLists[0] = lessonPlan.Warmup;
            exerciseLists[1] = lessonPlan.Round1;
            exerciseLists[2] = lessonPlan.Round2;
            exerciseLists[3] = lessonPlan.Round3;
            exerciseLists[4] = lessonPlan.CoolDown;
            for (int i = 0; i < categories; i++)
            {
                int count;
                if (exerciseLists[i] != null) count = exerciseLists[i].Count; else count = 0;

                for (int j = 0; j < count; j++)
                {
                    CourseExercise ce = exerciseLists[i][j];
                    string ID = $"{(int)ce.trainingInfo.splitKind},{(int)ce.trainingInfo.category},{ce.name},{(int)ce.trainingInfo.sets},{(int)ce.trainingInfo.reps}";
                    stream.WriteLine(ID);
                }
            }
            stream.Close();
        }

        private static void StoreIntoMyRoutinesList()
        {
            //StreamWriter stream = new StreamWriter(GlobalVar.myRoutinesFile);
            File.AppendAllText(GlobalVar.myRoutinesFile, lessonPlanOptions.routineName + Environment.NewLine);
        }

        private static void SavePDF()
        {
            string htmlFile = lessonPlanOptions.routineName + ".html";

            if (lessonPlanOptions.createPDF)
            {
                HtmlToPdf converter = new HtmlToPdf();
                string fullPath = Path.GetFullPath(htmlFile);
                PdfDocument doc = converter.ConvertUrl(fullPath);
                string fullFileName = lessonPlanOptions.routineName + ".pdf";
                doc.Save(fullFileName);
                doc.Close();
                Process.Start(fullFileName);
            }
        }

        private static void SetUpCurrentSaveDirectory(string name)
        {
            string filePath = Path.Combine(GlobalVar.applicationDataFolder, Path.GetFileNameWithoutExtension(name));
            Directory.CreateDirectory(filePath);
            Directory.SetCurrentDirectory(filePath);
        }

        private static void SaveHTMLFile(LessonPlanRoutine lessonPlan)
        {

            //Save html file.
            string fileName = lessonPlanOptions.routineName + ".html";
            SetUpCurrentSaveDirectory(fileName);
            WriteHtmlBeginningSection(fileName, "splits");

            //Warm Up
            if (lessonPlan.Warmup != null && lessonPlan.Warmup.Count != 0)
            {
                WriteHtmlExerciseSection(fileName, "Warm Up");
                for (int i = 0; i < lessonPlan.Warmup.Count; i++)
                {
                    WriteHtmlExercise(fileName, lessonPlan.Warmup[i]);
                }
            }

            //Round 1
            if (lessonPlan.Round1 != null && lessonPlan.Round1.Count != 0)
            {
                WriteHtmlExerciseSection(fileName, "Round 1");
                for (int i = 0; i < lessonPlan.Round1.Count; i++)
                {
                    WriteHtmlExercise(fileName, lessonPlan.Round1[i]);
                }
            }

            //Round 2
            if (lessonPlan.Round2 != null && lessonPlan.Round2.Count != 0)
            {
                WriteHtmlExerciseSection(fileName, "Round 2");
                for (int i = 0; i < lessonPlan.Round2.Count; i++)
                {
                    WriteHtmlExercise(fileName, lessonPlan.Round2[i]);
                }
            }

            //Round 3
            if (lessonPlan.Round3 != null && lessonPlan.Round3.Count != 0)
            {
                WriteHtmlExerciseSection(fileName, "Round 3");
                for (int i = 0; i < lessonPlan.Round3.Count; i++)
                {
                    WriteHtmlExercise(fileName, lessonPlan.Round3[i]);
                }
            }

            //Cool Down
            if (lessonPlan.CoolDown != null && lessonPlan.CoolDown.Count != 0)
            {
                WriteHtmlExerciseSection(fileName, "Cool Down");
                for (int i = 0; i < lessonPlan.CoolDown.Count; i++)
                {
                    WriteHtmlExercise(fileName, lessonPlan.CoolDown[i]);
                }
            }

            //Footer
            WriteHtmlEnd(fileName);

            //Execute file to display on default browser.
            if (lessonPlanOptions.openBrowser) Process.Start(fileName);
        }

        private static void WriteHtmlBeginningSection(string fileName, string title)
        {
            string[] htmlHead = new string[]
            {
                "<!DOCTYPE html>" + Environment.NewLine,
                "<html>\n" + Environment.NewLine,
                "<head>\n" + Environment.NewLine,
                "<style>\n" + Environment.NewLine,
                "#exercise {" + Environment.NewLine,
                "font - family: \"Trebuchet MS\", Arial, Helvetica, sans - serif;" + Environment.NewLine,
                "width: 100 %;" + Environment.NewLine,
                "min-width:1000px;" + Environment.NewLine,
                "max-width:1000px;" + Environment.NewLine,
                "border: 1px solid black;" + Environment.NewLine,
                "}" + Environment.NewLine,
                "#execise td, #exercise th, {" + Environment.NewLine,
                "border: 1px solid black; " + Environment.NewLine,
                "padding: 15px; " + Environment.NewLine,
                "}" + Environment.NewLine,
                "#exercise tr:nth-child(even){background-color: #f2f2f2;}" + Environment.NewLine,
                "#exercise tr:hover {background-color: lightblue;}" + Environment.NewLine,
                "#exercise th {" + Environment.NewLine,
                "padding-top: 12px;" + Environment.NewLine,
                "padding-bottom: 12px;" + Environment.NewLine,
                "text-align: center;" + Environment.NewLine,
                "background-color: #4CAF50;" + Environment.NewLine,
                "color: white;" + Environment.NewLine,
                "}" + Environment.NewLine,
                "</style>" + Environment.NewLine,
                "</head>" + Environment.NewLine,
                "<body>" + Environment.NewLine
            };
            for (int i = 0; i < htmlHead.Length; i++)
            {
                File.AppendAllText(fileName, htmlHead[i]);
            }
        }

        private static void WriteHtmlExerciseSection(string fileName, string title)
        {
            //style=\"min-width:1000px;\" style=\"max-width:1000px;\"
            string[] htmlSection = new string[]
            {

                "<table id=\"exercise\"  style=\"min-width:1000px;\" style=\"max-width:1000px;\">" + Environment.NewLine,
                "<tr>" + Environment.NewLine,
                "<th colspan=\"2\"> <H3>" + title +"</H3></th>" + Environment.NewLine,
                "</tr>" + Environment.NewLine,
            };
            for (int i = 0; i < htmlSection.Length; i++)
            {
                File.AppendAllText(fileName, htmlSection[i]);
            }
        }

        private static void WriteHtmlExercise(string fileName, CourseExercise exercise)
        {
            string pictureName = exercise.name + ".jpg";

            //string picturePath = "Splits Routine";
            //TODO: Add error handling here.
            exercise.image.Save(pictureName);
            string[] exerciseInfo = new string[]
            {
                "<tr>" + Environment.NewLine,
                "<td><img src = \""+ pictureName + "\"" + " height = \"130\"></td>" + Environment.NewLine,
                "<td>" + Environment.NewLine,
                "<p>" + exercise.name + "</p>" + Environment.NewLine,
                "<p>" + exercise.trainingInfo.description + "</p>" + Environment.NewLine,
                "<p>" + "Sets: " + exercise.trainingInfo.sets + " - Reps: " + exercise.trainingInfo.reps + "</p>" + Environment.NewLine,
                "<p>" + exercise.trainingInfo.notes + "</p>" + Environment.NewLine,
                "</td>" + Environment.NewLine,
                "</tr>" + Environment.NewLine,
            };
            for (int i = 0; i < exerciseInfo.Length; i++)
            {
                File.AppendAllText(fileName, exerciseInfo[i]);
            }
        }L

        private static void WriteHtmlEnd(string fileName)
        {
            string[] htmlEnd = new string[]
                {
                    "</table>" + Environment.NewLine,
                    "</body>" + Environment.NewLine,
                    "</html>" + Environment.NewLine
                };
            for (int i = 0; i < htmlEnd.Length; i++)
            {
                File.AppendAllText(fileName, htmlEnd[i]);
            }
        }
    }
}
