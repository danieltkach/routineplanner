using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Routine_Planner
{   
    public enum SplitKind
    {
        All,
        Side,
        OpenFront,
        TrueFront,
    };

    public enum Difficulty
    {
        All,
        Beginner,
        Intermediate,
        Advanced
    };

    public enum Category
    {
        WarmUp,
        KinesiologicalStretching,
        MovementHabituation,
        Strength,
        CoolDown,
        Mobility
    };

    public struct TrainingInfo
    {
        public Category category;
        public SplitKind splitKind;
        public Difficulty difficulty;
        public int sets;
        public int reps;
        public string notes;
        public string description;
        public bool isCompound;
        public bool mustSwitchDirection;
        public bool mustSwitchSides;
    }

    public enum CoolDown
    {
        None,
        Beginner,
        Intermediate,
        Advanced
    };

    public struct CourseExercise
    {
        public string ID;
        public string name;
        public TrainingInfo trainingInfo;
        public Image image;
    }

    public struct LessonPlanRoutine
    {
        public List<CourseExercise> Warmup;
        public List<CourseExercise> Round1;
        public List<CourseExercise> Round2;
        public List<CourseExercise> Round3;
        public List<CourseExercise> CoolDown;
    }

    struct RoundIndexes
    {
        public List<int> KinesiologicalStretching;
        public List<int> MovementHabituation;
        public List<int> Strength;
    }

    struct RoutineIndexes
    {
        public List<int> Warmup;
        public List<int> Circles;
        public RoundIndexes[] Round;
        public List<int> CoolDown;
    }

    public enum Sides
    {
        right,
        left,
        rightleft,
        both,
        single
    }
    

    public static class GlobalVar
    {
        public const int Warmup = 0;
        public const int Round1 = 1;
        public const int Round2 = 2;
        public const int Round3 = 3;
        public const int CoolDown = 4;
        public static Random r = new Random();
        public const int jointRotationsIndex = 0;
        public const int glutesRotationIndex = 1;
        public static string applicationDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "EasySplits Routines");
        public static string myRoutinesFile = Path.Combine(applicationDataFolder, "myRoutines.txt");
        public const string ProgramExtension = ".rplnr";
    }
}
