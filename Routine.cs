using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Routine_Planner
{
    class Circles
    {
        public List<CourseExercise> PickedCircles { get; set; }
        public List<int> indexes;

        public Circles(SplitKind split, Difficulty difficulty)
        {
            PickedCircles = new List<CourseExercise>();
            var ListBySplitAndCategory = ExercisesLoader.exercisesMatrix[(int)split, (int)Category.WarmUp];
            indexes = new List<int>();
            //Determine which circles are going to be included in the routine.

            switch (difficulty)
            {
                case Difficulty.All:
                    switch (GlobalVar.r.Next(1, 4))
                    {
                        case 1:
                            //Knee bend on the floor.
                            PickedCircles.AddRange(ListBySplitAndCategory.Where(ex => ex.name.ToLower().Contains("circle") && !ex.name.ToLower().Contains("straight")));
                            indexes.Add(1);
                            break;
                        case 2:
                            //Leg straight.
                            PickedCircles.AddRange(ListBySplitAndCategory.Where(ex => ex.name.ToLower().Contains("straight")));
                            indexes.Add(2);
                            break;
                        case 3:
                            //Both knee bend and leg straight.
                            PickedCircles.AddRange(ListBySplitAndCategory.Where(ex => ex.name.ToLower().Contains("circle")));
                            indexes.Add(3);
                            break;
                    }
                    break;
                case Difficulty.Beginner:
                    PickedCircles.AddRange(ListBySplitAndCategory.Where(ex => ex.name.ToLower().Contains("circle") && !ex.name.ToLower().Contains("straight")));
                    indexes.Add(1);
                    break;
                case Difficulty.Intermediate:
                    PickedCircles.AddRange(ListBySplitAndCategory.Where(ex => ex.name.ToLower().Contains("circle") && !ex.name.ToLower().Contains("straight")));
                    indexes.Add(1);
                    if (GlobalVar.r.Next(0, 2) == 0)
                    {
                        PickedCircles.AddRange(ListBySplitAndCategory.Where(ex => ex.name.ToLower().Contains("straight")));
                        indexes.Add(2);
                    }
                    break;
                case Difficulty.Advanced:
                    if (GlobalVar.r.Next(0, 2) == 1)
                    {
                        PickedCircles.AddRange(ListBySplitAndCategory.Where(ex => ex.name.ToLower().Contains("circle") && !ex.name.ToLower().Contains("straight")));
                        indexes.Add(1);
                    }
                    PickedCircles.AddRange(ListBySplitAndCategory.Where(ex => ex.name.ToLower().Contains("straight")));
                    indexes.Add(2);
                    break;
            }
            //Glutes circles.
            bool includeGlutes;
            includeGlutes = GlobalVar.r.Next(0, 2) == 1;
            if (includeGlutes)
            {
                PickedCircles.AddRange(ExercisesLoader.exercisesMatrix[(int)SplitKind.All, (int)Category.Mobility].Where(ex => ex.name.ToLower().Contains("glutes circle")));
                indexes.Add(1);
            }
            else
            {
                indexes.Add(0); //No glutes exercises.
            }
        }
    }

    public class Routine
    {
        private SplitKind splitKind { get; set; }
        private Difficulty difficulty { get; set; }
        private int rounds { get; set; }
        private bool includeZejax { get; set; }

        private string routineName = LessonPlanWriter.lessonPlanOptions.routineName;
        //This is the table that holds all the lists with the exercises in the course.
        private List<CourseExercise>[,] exercisesMatrix = new List<CourseExercise>[4, 5];

        public List<CourseExercise>[] exercisesList { get; set; }
        //Stores the exercises as indexes so that the routine can be saved in a simple txt and retrieved easily into the program.
        private RoutineIndexes Indexes = new RoutineIndexes();
        
        //Random random = new Random();
        
        public void LoadUserInterfaceLists(TabControl tabsRoutine)
        {            
            for (int i = 0; (i < tabsRoutine.TabCount); i++)
            {
                if (exercisesList[i] == null) continue;
                tabsRoutine.SelectedIndex = i;
                (tabsRoutine.TabPages[i].Controls[0] as ListBox).Items.Clear();
                for (int j = 0; j < exercisesList[i].Count; j++)
                {
                    (tabsRoutine.TabPages[i].Controls[0] as ListBox).Items.Add(exercisesList[i][j].name);
                }                
            }
        }

        /*
        public void SaveTxtRoutine(TabControl tabsRoutine)
        {
            string applicationPath = GlobalVar.applicationDataFolder;
            string fileName = routineName + ".txt";
            string savedFileName = Path.Combine(applicationPath, fileName);
            StreamWriter stream = new StreamWriter(savedFileName);
            stream.WriteLine($"{splitKind},");
            for (int i = 0; (i < tabsRoutine.TabCount); i++)
            {
                var list = tabsRoutine.TabPages[i].Controls[0] as ListBox;
                stream.WriteLine(list.Items.Count);
                if (exercisesList[i] == null)
                {
                    stream.WriteLine("empty");
                    continue;
                }
                tabsRoutine.SelectedIndex = i;

                for (int j = 0; j < exercisesList[i].Count; j++)
                {
                    string exerciseName = list.Items[j].ToString();
                    stream.WriteLine(exerciseName);
                }
            }
            stream.Close();
        }
        */

        //public void LoadTxtRoutine(string fileName)
        //{
        //    LessonPlanRoutine lessonPlan = new LessonPlanRoutine();
        //    StreamReader stream = new StreamReader(fileName);
        //    int[] sectionExCount = new int[5];

        //    sectionExCount[GlobalVar.Warmup] = int.Parse(stream.ReadLine());
        //    for (int i = 0; i < sectionExCount[GlobalVar.Warmup]; i++)
        //    {
        //        string exName = stream.ReadLine();
        //        var ceList = exercisesMatrix[GlobalVar.Warmup, (int)Category.Mobility].Where(ex => ex.name.ToLower().Contains(exName));
        //        lessonPlan.Warmup.Add(ceList.ToArray()[0]);
        //    }

        //}

        private int[] RandomExerciseIndexes(int howmany, int max)
        {
            //const int min = 0;
            int[] numbers = new int[howmany];
            int number;
            int counter = 0;

            while (counter < howmany)
            {
                do
                {
                    number = GlobalVar.r.Next(0, max);
                } while (numbers.Contains<int>(number));
                numbers[counter] = number;
                counter++;
            }            
            return numbers;
        }

        private void AddWarmupExercises(SplitKind split, Difficulty difficulty)
        {
            //Add joint rotations.
            CourseExercise jointRotations = new CourseExercise();
            jointRotations = exercisesMatrix[(int)SplitKind.All, (int)Category.Mobility][GlobalVar.jointRotationsIndex];
            exercisesList[(int)Category.WarmUp] = new List<CourseExercise>();
            exercisesList[(int)Category.WarmUp].Add(jointRotations);
            Indexes.Warmup = new List<int>();
            //routineIndexes.Warmup.Add(GlobalVar.jointRotationsIndex);

            //Add conditioning exercises.
            bool isCompound = true;
            int[] warmupIndexes1 = AddExercisesToRound(GlobalVar.Warmup, SplitKind.All, Category.WarmUp, 1 + GlobalVar.r.Next(0, 3), isCompound);
            Indexes.Warmup.AddRange(warmupIndexes1);
            int[] warmupIndexes2 = AddExercisesToRound(GlobalVar.Warmup, SplitKind.All, Category.WarmUp, 1 + GlobalVar.r.Next(0, 3), !isCompound);
            Indexes.Warmup.AddRange(warmupIndexes2);

            //Add warmup circles.   
            Circles c = new Circles(split, difficulty);
            var warmupCircles = c.PickedCircles;
            exercisesList[(int)Category.WarmUp].AddRange(warmupCircles);
            Indexes.Circles = new List<int>();
            Indexes.Circles = c.indexes;
        }

        private int[] AddExercisesToRound(int round, SplitKind chosenSplit, Category category, int howmanyIneed, bool isCompound = false)
        {
            CourseExercise courseExercise = new CourseExercise();
            if (exercisesList[round] == null) exercisesList[round] = new List<CourseExercise>();

            List<CourseExercise> ListToPickFrom = new List<CourseExercise>();
            ListToPickFrom = ExercisesLoader.SelectExercisesFromMatrix(chosenSplit, category, difficulty, includeZejax, isCompound);
            int howmanyArethere = ListToPickFrom.Count;
            
            //int howmanyArethere = exercisesMatrix[(int)chosenSplit, (int)category].Count;

            int[] randomNumbers = RandomExerciseIndexes(howmanyIneed, howmanyArethere);
            for (int i = 0; i < howmanyIneed; i++)
            {
                //courseExercise = exercisesMatrix[(int)chosenSplit, (int)category][randomNumbers[i]];
                courseExercise = ListToPickFrom[randomNumbers[i]];
                exercisesList[round].Add(courseExercise);
            }
            return randomNumbers;
        }

        private void AddCoolDownExercises(Difficulty difficulty)
        {
            List<CourseExercise> coolDown = new List<CourseExercise>();
            if (difficulty == Difficulty.All)
            {
                difficulty = (Difficulty)GlobalVar.r.Next(1, 4);
            }
            coolDown = exercisesMatrix[(int)SplitKind.All, (int)Category.CoolDown].Where(ex => ex.name.Contains($"L{(int)difficulty}")).ToList();
            exercisesList[(int)Category.CoolDown] = new List<CourseExercise>();
            exercisesList[(int)Category.CoolDown].AddRange(coolDown);
            Indexes.CoolDown = new List<int>();
            Indexes.CoolDown.Add((int)difficulty);
        }

        private void CreateRound(int round, SplitKind chosenSplit, Difficulty difficulty)
        {
            int[] KinesiologicalStretching = AddExercisesToRound(round, chosenSplit, Category.KinesiologicalStretching, 3 + GlobalVar.r.Next(0, 3));
            int[] MovementHabituation = AddExercisesToRound(round, chosenSplit, Category.MovementHabituation, GlobalVar.r.Next(1, 3));
            int[] Strength = AddExercisesToRound(round, chosenSplit, Category.Strength, 1);
            Indexes.Round[round].KinesiologicalStretching = new List<int>();
            Indexes.Round[round].MovementHabituation = new List<int>();
            Indexes.Round[round].Strength = new List<int>();
            Indexes.Round[round].KinesiologicalStretching.AddRange(KinesiologicalStretching);
            Indexes.Round[round].MovementHabituation.AddRange(MovementHabituation);
            Indexes.Round[round].Strength.AddRange(Strength);
        }

        public static string GetRoutineDateName()
        {
            return (DateTime.Now.ToString().Replace('/', '-').Replace(':', '.'));
        }

        public Routine(List<CourseExercise>[,] exercisesMatrix, SplitKind chosenSplit, Difficulty chosenLevel, int howManyRounds, CoolDown coolDownLevel = CoolDown.Beginner, bool includeZejax = false)
        {            
            this.exercisesMatrix = exercisesMatrix;
            this.difficulty = chosenLevel;
            rounds = howManyRounds;
            exercisesList = new List<CourseExercise>[5];
            Indexes.Round = new RoundIndexes[4];
            if (chosenSplit == SplitKind.All) splitKind = (SplitKind)GlobalVar.r.Next(1, 4); else splitKind = chosenSplit;
            this.includeZejax = includeZejax;

            // Add exercises to each round \\
            AddWarmupExercises(splitKind, difficulty);
            Indexes.Round = new RoundIndexes[4];
            for (int round = 1; round <= howManyRounds; round++)
            {
                CreateRound(round, chosenSplit, difficulty);
            }
            bool includeCoolDown = coolDownLevel != CoolDown.None;
            if (includeCoolDown)
            {
                AddCoolDownExercises(difficulty);
            }
        }

        /*
        private void SaveRoutineIndexesToFile(string routineName)
        {
            string applicationPath = GlobalVar.applicationDataFolder;
            string fileName = routineName + " -indexes.txt";
            string savedFileName = Path.Combine(applicationPath, fileName);
            StreamWriter stream = new StreamWriter(savedFileName);

            //Conditioning
            stream.WriteLine("Conditioning");
            int waCount = Indexes.Warmup.Count;
            for (int i = 0; i < waCount; i++)
            {
                stream.Write(Indexes.Warmup[i]);
                stream.Write(i < waCount - 1 ? "," : "");
            }
            stream.WriteLine();

            //Circles
            stream.WriteLine("Circles");
            int cCount = Indexes.Circles.Count;
            for (int i = 0; i < cCount; i++)
            {
                stream.Write(Indexes.Circles[i]);
                stream.Write(i < cCount - 1 ? "," : "");
            }
            stream.WriteLine();

            //Rounds 1-3
            for (int i = 1; i <= rounds; i++)
            {
                stream.WriteLine($"Round{i}");
                List<int>[] list = new List<int>[3];
                list[0] = new List<int>();
                list[1] = new List<int>();
                list[2] = new List<int>();
                list[0] = Indexes.Round[i].KinesiologicalStretching;
                list[1] = Indexes.Round[i].MovementHabituation;
                list[2] = Indexes.Round[i].Strength;
                for (int lst = 0; lst < 3; lst++)
                {
                    int j = 0;
                    int rCount = list[lst].Count;
                    while (j < rCount)
                    {
                        stream.Write(list[lst][j]+(j < rCount-1?",":""));
                        j++;
                    }
                    stream.Write(lst<2?"/" : "");
                }
                stream.WriteLine();                
            }
            
            //Cool Down
            stream.WriteLine("Cool Down");
            stream.WriteLine((int)difficulty);
            stream.Close();
        }
        */

    }
}
