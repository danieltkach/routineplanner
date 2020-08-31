using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Routine_Planner
{
    public static class ExercisesLoader
    {
        public static int numberOfExercises = 0;
        public static List<CourseExercise>[,] exercisesMatrix = new List<CourseExercise>[4, 6];

        /*
        public static void LoadTxtRoutine(string fileName)
        {
            LessonPlanRoutine lessonPlan = new LessonPlanRoutine();
            lessonPlan.Warmup = new List<CourseExercise>();
            lessonPlan.Round1 = new List<CourseExercise>();
            lessonPlan.Round2 = new List<CourseExercise>();
            lessonPlan.Round3 = new List<CourseExercise>();
            lessonPlan.CoolDown = new List<CourseExercise>();

            StreamReader stream = new StreamReader(fileName);

            int[] sectionExCount = new int[5];
            sectionExCount[GlobalVar.Warmup] = int.Parse(stream.ReadLine());
            for (int i = 0; i < sectionExCount[GlobalVar.Warmup]; i++)
            {
                string exName = stream.ReadLine();
                var ceList = exercisesMatrix[GlobalVar.Warmup, (int)Category.Mobility].Where(ex => ex.name.Contains(exName));
                lessonPlan.Warmup.Add(ceList.ToArray()[0]);
            }
            //TODO: do the same for the other sections (rounds1,2,3, cooldown etc).
        }
        */

        public static List<CourseExercise> SelectExercisesFromMatrix(SplitKind split, Category category, Difficulty difficulty, bool includeZejax = false, bool isCompound = false)
        {
            List<CourseExercise> selectedExercises = new List<CourseExercise>();
            selectedExercises = ExercisesLoader.exercisesMatrix[(int)split, (int)category];

            if (category == Category.WarmUp)
            {
                if (isCompound)
                {
                    selectedExercises = selectedExercises.Where(exercise => exercise.trainingInfo.isCompound == true).ToList();
                }
                else
                {
                    selectedExercises = selectedExercises.Where(exercise => exercise.trainingInfo.isCompound == false).ToList();
                }
            }

            if (difficulty == Difficulty.All)
            {
                if (includeZejax)
                {
                    selectedExercises = ExercisesLoader.exercisesMatrix[(int)split, (int)category];
                }
                else if (!includeZejax)
                {
                    selectedExercises = selectedExercises.Where(exercise => !(exercise.name.ToLower().Contains("zejax"))).ToList();
                }
            }
            else if (difficulty != Difficulty.All)
            {
                if (includeZejax)
                {
                    selectedExercises = selectedExercises.Where(exercise => (exercise.trainingInfo.difficulty == difficulty)).ToList();
                }
                else
                {
                    selectedExercises = selectedExercises.Where(exercise => (exercise.trainingInfo.difficulty == difficulty) && !(exercise.name.ToLower().Contains("zejax"))).ToList();
                }
            }

            if (selectedExercises.Count == 0)
            {
                selectedExercises = ExercisesLoader.exercisesMatrix[(int)split, (int)category];
            }
            return selectedExercises;
        }

        private static void LoadExerciseIntoMatrix(string name, string description, Image image, TrainingInfo trainingInfo)
        {
            //Inputs all the exercise information into a "courseExercise" object.
            CourseExercise courseExercise = new CourseExercise();
            courseExercise.name = name;
            courseExercise.trainingInfo.category = trainingInfo.category;
            courseExercise.trainingInfo.splitKind = trainingInfo.splitKind;
            courseExercise.trainingInfo.description = "Difficulty: " + trainingInfo.difficulty + Environment.NewLine + description;
            courseExercise.image = image;
            courseExercise.trainingInfo.reps = trainingInfo.reps;
            courseExercise.trainingInfo.sets = trainingInfo.sets;
            courseExercise.trainingInfo.notes = trainingInfo.notes;
            courseExercise.trainingInfo.difficulty = trainingInfo.difficulty;
            courseExercise.trainingInfo.isCompound = trainingInfo.isCompound;
            courseExercise.ID = $"{trainingInfo.splitKind},{trainingInfo.category},{name},{trainingInfo.sets},{trainingInfo.reps}";
            courseExercise.trainingInfo.mustSwitchSides = trainingInfo.mustSwitchSides;
            courseExercise.trainingInfo.mustSwitchDirection = trainingInfo.mustSwitchDirection;

            //Put this exercise object into the global matrix that holds all the exercises in the course.
            if (exercisesMatrix[(int)trainingInfo.splitKind, (int)trainingInfo.category] == null)
            {
                exercisesMatrix[(int)trainingInfo.splitKind, (int)trainingInfo.category] = new List<CourseExercise>();
            }
            exercisesMatrix[(int)trainingInfo.splitKind, (int)trainingInfo.category].Add(courseExercise);
            numberOfExercises++;
        }

        public static void LoadExercisesFromResources()
        {            
            TrainingInfo trainingInfo;

            // WARM UP AND CONDITIONING \\            
            // Split specific warmup circles
            
            trainingInfo.difficulty = Difficulty.All;
            trainingInfo.sets = 1;
            trainingInfo.reps = 10;
            trainingInfo.description = "Do as many as you feel your body needs. You can do the joint rotations while jogging in place, while on a stance, sitting, lying down etc.";
            trainingInfo.notes = "";
            trainingInfo.isCompound = false;
            trainingInfo.mustSwitchDirection = true; 
            trainingInfo.mustSwitchSides = true;
            //All Splits

            //General mobility (joint rotations and glutes rotations).
            trainingInfo.category = Category.Mobility;
            trainingInfo.splitKind = SplitKind.All;
            LoadExerciseIntoMatrix("Joint Rotations", trainingInfo.description, Properties.Resources.Joint_Rotations, trainingInfo);
            LoadExerciseIntoMatrix("Glutes Circles", trainingInfo.description, Properties.Resources.Glutes_Circles, trainingInfo);
            //Warmup Circles
            trainingInfo.category = Category.WarmUp;
            //Side Split
            trainingInfo.splitKind = SplitKind.Side;
            LoadExerciseIntoMatrix("Adductor Circles", trainingInfo.description, Properties.Resources.Adductor_Circles_Knee_Down, trainingInfo);
            LoadExerciseIntoMatrix("Adductor Circles Leg Straight", trainingInfo.description, Properties.Resources.Adductors_Circles_Leg_Straight, trainingInfo);
            //Open Front
            trainingInfo.splitKind = SplitKind.OpenFront;
            LoadExerciseIntoMatrix("Adductor Circles", trainingInfo.description, Properties.Resources.Adductor_Circles_Knee_Down, trainingInfo);
            LoadExerciseIntoMatrix("Adductor Circles Leg Straight", trainingInfo.description, Properties.Resources.Adductors_Circles_Leg_Straight, trainingInfo);
            LoadExerciseIntoMatrix("Hamstrings Circles Knee Down", trainingInfo.description, Properties.Resources.Hamstrings_Circles_Knee_Down, trainingInfo);
            LoadExerciseIntoMatrix("Hamstrings Circles Leg Straight", trainingInfo.description, Properties.Resources.Hamstrings_Circles_Straight_Leg, trainingInfo);
            //True Front
            trainingInfo.splitKind = SplitKind.TrueFront;
            LoadExerciseIntoMatrix("Hamstrings Circles Knee Down", trainingInfo.description, Properties.Resources.Hamstrings_Circles_Knee_Down, trainingInfo);
            LoadExerciseIntoMatrix("Hamstrings Circles Leg Straight", trainingInfo.description, Properties.Resources.Hamstrings_Circles_Straight_Leg, trainingInfo);
            LoadExerciseIntoMatrix("Hip Flexors Circles Knee Down", trainingInfo.description, Properties.Resources.Hip_Flexor_Circles_Knee_Down, trainingInfo);
            LoadExerciseIntoMatrix("Hip Flexors Circles Leg Straight", trainingInfo.description, Properties.Resources.Hip_Flexor_Circles_Leg_Straight, trainingInfo);

            //Conditioning
            trainingInfo.category = Category.WarmUp;
            trainingInfo.splitKind = SplitKind.All;
            trainingInfo.sets = 1;
            trainingInfo.reps = 25;
            trainingInfo.notes = "";
            trainingInfo.description = "";
            trainingInfo.mustSwitchDirection = false;
            trainingInfo.mustSwitchSides = true;
            // Level 1
            trainingInfo.difficulty = Difficulty.Beginner;
            trainingInfo.isCompound = true;
            LoadExerciseIntoMatrix("L1 Knee Lifts", "Keep your spine straight, lift your knee alternating sides as high as you can without using momentum.", Properties.Resources.Knee_Lifts, trainingInfo);
            trainingInfo.mustSwitchSides = false;
            LoadExerciseIntoMatrix("L1 Squats", "", Properties.Resources.Squats, trainingInfo);
            LoadExerciseIntoMatrix("L1 Dead Lifts", "Move from your hips keeping your back neutral.", Properties.Resources.Dead_Lifts, trainingInfo);
            trainingInfo.isCompound = false;
            LoadExerciseIntoMatrix("L1 Toe Presses", "", Properties.Resources.Toe_Presses, trainingInfo);
            LoadExerciseIntoMatrix("L1 Toe Lifts", "", Properties.Resources.Toe_Lifts, trainingInfo);
            trainingInfo.mustSwitchSides = true;
            LoadExerciseIntoMatrix("L1 Adductor Flies", "", Properties.Resources.Adductor_Flies, trainingInfo);
            LoadExerciseIntoMatrix("L1 Abductor Flies", "Lift your leg to the side and hold for a second.", Properties.Resources.Abductor_Flies, trainingInfo);
            trainingInfo.mustSwitchSides = false;
            LoadExerciseIntoMatrix("L1 Crunches", "Lift your upper back off the floor.", Properties.Resources.Crunches, trainingInfo);
            trainingInfo.isCompound = true;
            LoadExerciseIntoMatrix("L1 Planks", "", Properties.Resources.Planks, trainingInfo);
            LoadExerciseIntoMatrix("L1 Push Ups", "", Properties.Resources.Push_Ups, trainingInfo);
            LoadExerciseIntoMatrix("L1 Elastic Band Rows", "Sit straight, pull back.", Properties.Resources.Band_Rows, trainingInfo);
            trainingInfo.mustSwitchSides = true;
            LoadExerciseIntoMatrix("L1 ZejaX Rows", "", Properties.Resources.Zejax_Rows, trainingInfo);
            // Level 2
            trainingInfo.difficulty = Difficulty.Intermediate;
            trainingInfo.isCompound = true;
            trainingInfo.mustSwitchSides = false;
            LoadExerciseIntoMatrix("L2 Wide Squats", "", Properties.Resources.Wide_Squats, trainingInfo);
            trainingInfo.mustSwitchSides = true;
            LoadExerciseIntoMatrix("L2 Step Back Lunges", "", Properties.Resources.Step_Back_Lounges, trainingInfo);
            LoadExerciseIntoMatrix("L2 Lunges", "", Properties.Resources.Lounges, trainingInfo);
            trainingInfo.mustSwitchSides = false;
            LoadExerciseIntoMatrix("L2 Elevated Half Bridge", "", Properties.Resources.Half_Bridge_Elevated, trainingInfo);
            trainingInfo.isCompound = false;
            trainingInfo.mustSwitchSides = true;
            LoadExerciseIntoMatrix("L2 Hip Flexors Flies", "", Properties.Resources.Hip_Flexors_Flies, trainingInfo);
            trainingInfo.mustSwitchSides = false;
            LoadExerciseIntoMatrix("L2 Elevated Toe Presses", "", Properties.Resources.Elevated_Toe_Presses, trainingInfo);
            LoadExerciseIntoMatrix("L2 Incline Toe Lifts", "", Properties.Resources.Incline_Toe_Lifts, trainingInfo);
            trainingInfo.mustSwitchSides = true;
            LoadExerciseIntoMatrix("L2 Adductors + Abductors", "Lift top leg then keeping it up lift the bottom leg.", Properties.Resources.Adductors___Abductors, trainingInfo);
            LoadExerciseIntoMatrix("L2 Elevated Adductors", "", Properties.Resources.Elevated_Adductors, trainingInfo);
            trainingInfo.isCompound = true;
            trainingInfo.mustSwitchSides = false;
            LoadExerciseIntoMatrix("L2 Diamond Push Ups", "", Properties.Resources.Diamond_Push_Ups, trainingInfo);
            trainingInfo.mustSwitchSides = true;
            LoadExerciseIntoMatrix("L2 ZejaX One Arm Row", "", Properties.Resources.Zejax_One_Arm_Row, trainingInfo);
            // Level 3
            trainingInfo.difficulty = Difficulty.Advanced;
            trainingInfo.isCompound = true;
            trainingInfo.mustSwitchSides = true;
            LoadExerciseIntoMatrix("L3 Step Up", "", Properties.Resources.Step_Up, trainingInfo);
            LoadExerciseIntoMatrix("L3 Sideways Lounges", "", Properties.Resources.Lounges_Sideways, trainingInfo);
            trainingInfo.isCompound = false;
            LoadExerciseIntoMatrix("L3 Abductor Flies (Standing)", "Lift your leg to the side and hold for a second.", Properties.Resources.Abductor_Flies_Standing, trainingInfo);
            LoadExerciseIntoMatrix("L3 Single Leg Toe Presses", "", Properties.Resources.Toe_Presses_Single_Leg, trainingInfo);
            trainingInfo.isCompound = true;
            LoadExerciseIntoMatrix("L3 Stiff Leg Dead Lift", "", Properties.Resources.Stiff_Leg_Dead_Lifts, trainingInfo);
            LoadExerciseIntoMatrix("L3 Elevated One Leg Half Bridges", "", Properties.Resources.Elevated_One_Leg_Half_Bridges, trainingInfo);
            trainingInfo.isCompound = false;
            trainingInfo.mustSwitchSides = false;
            LoadExerciseIntoMatrix("L3 Windshield Wipers Legs Bend", "", Properties.Resources.Windshield_Wipers_Legs_Bend, trainingInfo);
            LoadExerciseIntoMatrix("L3 Winshield Wipers Legs Straight", "", Properties.Resources.Windshield_Wipers_Legs_Straight, trainingInfo);
            trainingInfo.isCompound = true;
            LoadExerciseIntoMatrix("L3 Overhead Push Ups", "", Properties.Resources.Overhead_Push_Ups, trainingInfo);
            LoadExerciseIntoMatrix("L3 Elevated Push Ups", "", Properties.Resources.Elevated_Push_Ups, trainingInfo);
            LoadExerciseIntoMatrix("L3 ZejaX Pony Pulls", "", Properties.Resources.Zejax_Pony_Pulls, trainingInfo);
            trainingInfo.mustSwitchSides = true;
            LoadExerciseIntoMatrix("L3 ZejaX Incline Pullups", "", Properties.Resources.Zejax_Incline_Pulls_Ups, trainingInfo);
            //---------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // KINESIOLOGICAL STRETCHING \\
            trainingInfo.category = Category.KinesiologicalStretching;
            trainingInfo.sets = 2;
            trainingInfo.reps = 5;
            trainingInfo.difficulty = Difficulty.All;
            trainingInfo.mustSwitchSides = true;
            // Side Split
            trainingInfo.splitKind = SplitKind.Side;
            LoadExerciseIntoMatrix("~Reverence~", "Target: Reach chest to floor. Leverage: Lift heel.", Properties.Resources.Reverence, trainingInfo);
            LoadExerciseIntoMatrix("~Integrity~", "Target: Groin to floor. Leverage: Extend hips, contract abs and glutes.", Properties.Resources.Integrity, trainingInfo);
            LoadExerciseIntoMatrix("~Content~", "Target: Knee extension and hip flexion, kicking higher up and above more and more. Leverage: Knee extension, move only from your knee", Properties.Resources.Content, trainingInfo);
            LoadExerciseIntoMatrix("~Esteem~", "Target: Reach the floor with your chest. Leverage: Internal or external hip rotation.", Properties.Resources.Esteem, trainingInfo);
            LoadExerciseIntoMatrix("~Purity~", "Target: Reach your chest to the floor. Leverage: push leg back and away, creating external rotation and abduction.",
                Properties.Resources.Purity, trainingInfo);
            LoadExerciseIntoMatrix("~Equilibrium~", "Target: Hip abduction, reaching the floor with your groin. Leverage: external hip rotation, rotate your leg contracting glutes without moving you torso.",
                Properties.Resources.Equilibrium, trainingInfo);
            trainingInfo.mustSwitchSides = false;
            LoadExerciseIntoMatrix("~Balance~", "Target: Reach the floor with your chest. Leverage: Move your torso sideways alternating sides. Keep your spine straight.",
                Properties.Resources.Balance, trainingInfo);
            // Open Front Split
            trainingInfo.mustSwitchSides = true;
            trainingInfo.splitKind = SplitKind.OpenFront;
            LoadExerciseIntoMatrix("~Deference~", "", Properties.Resources.OFS_KS_Deference, trainingInfo);
            LoadExerciseIntoMatrix("~Integrity~", "Target: Groin to floor. Leverage: Extend hips, contract abs and glutes.",
               Properties.Resources.Integrity, trainingInfo);
            LoadExerciseIntoMatrix("~Content~ (chair variation)", "Target: Knee extension and hip flexion, kicking higher up and above more and more. Leverage: Knee extension, move only from your knee",
                Properties.Resources.OFS_KS_Content__chair_variation_, trainingInfo);
            LoadExerciseIntoMatrix("~Equilibrium~", "Target: Hip abduction, reaching the floor with your groin. Leverage: external hip rotation, rotate your leg contracting glutes without moving you torso.", Properties.Resources.Equilibrium, trainingInfo);
            LoadExerciseIntoMatrix("~Certitude~", "Target: Hip abduction, reaching the floor with your groin. Leverage: external hip rotation.", Properties.Resources.OFS_KS_Certitude, trainingInfo);
            LoadExerciseIntoMatrix("~Courage~", "Target: Hip adduction combined with hip extension. Leverage: external hip rotation. Very similar looking to ~Certitude~ but very different. Here we do ADDUCTION instead.", Properties.Resources.OFS_KS_Courage, trainingInfo);
            LoadExerciseIntoMatrix("~Amity~", "", Properties.Resources.OFS_KS_Amity, trainingInfo);
            LoadExerciseIntoMatrix("~Exuberance~", "Target: Internal hip rotation. Leverage: Hip Abduction.", Properties.Resources.OFS_KS_Exuberance, trainingInfo);
            // True Front Split
            trainingInfo.splitKind = SplitKind.TrueFront;
            LoadExerciseIntoMatrix("~Ease~", "", Properties.Resources.Ease, trainingInfo);
            LoadExerciseIntoMatrix("~Appeal~", "", Properties.Resources.Appeal, trainingInfo);
            LoadExerciseIntoMatrix("~Reverence~", "", Properties.Resources.TFS_Reverence, trainingInfo);
            LoadExerciseIntoMatrix("~Peace~", "", Properties.Resources.Peace, trainingInfo);
            LoadExerciseIntoMatrix("~Kindness~", "", Properties.Resources.Kindness, trainingInfo);
            LoadExerciseIntoMatrix("~Harmony~", "", Properties.Resources.Harmony, trainingInfo);
            LoadExerciseIntoMatrix("~Integrity~", "", Properties.Resources.TFS_Integrity, trainingInfo);
            LoadExerciseIntoMatrix("~Equilibrium~", "", Properties.Resources.TFS_Equilibrium, trainingInfo);
            LoadExerciseIntoMatrix("~Security~", "", Properties.Resources.Security, trainingInfo);
            LoadExerciseIntoMatrix("~Stability~", "", Properties.Resources.Stability, trainingInfo);
            LoadExerciseIntoMatrix("~Surety~", "", Properties.Resources.Surety, trainingInfo);
            //---------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // MOVEMENT AND HABITUATION TECHNIQUES \\
            trainingInfo.category = Category.MovementHabituation;
            trainingInfo.difficulty = Difficulty.All;
            trainingInfo.sets = 1;
            trainingInfo.reps = 10;
            // Side Split
            trainingInfo.splitKind = SplitKind.Side;
            trainingInfo.difficulty = Difficulty.Beginner;
            trainingInfo.mustSwitchSides = false;
            LoadExerciseIntoMatrix("Front-Horse Stance Transitions", "Keep your hips at the same level and only move from one hip.", Properties.Resources.Front_Horse_Stance_Transitions, trainingInfo);
            trainingInfo.mustSwitchSides = true;
            LoadExerciseIntoMatrix("Side Leg Circles", "", Properties.Resources.Side_Leg_Circles, trainingInfo);
            LoadExerciseIntoMatrix("Off-Side Frog", "", Properties.Resources.Off_Side_Frog, trainingInfo);
            trainingInfo.difficulty = Difficulty.Intermediate;
            LoadExerciseIntoMatrix("Half Frog Lifts", "", Properties.Resources.Half_Frog_Lifts, trainingInfo);
            LoadExerciseIntoMatrix("Extended Frog Lifts", "", Properties.Resources.Extended_Frog_Lifts, trainingInfo);
            trainingInfo.mustSwitchSides = false;
            LoadExerciseIntoMatrix("Whirlwind", "", Properties.Resources.Whirlwind, trainingInfo);
            trainingInfo.difficulty = Difficulty.Advanced;
            trainingInfo.mustSwitchSides = true;
            LoadExerciseIntoMatrix("Prone First Position Kicks", "", Properties.Resources.Prone_First_Position_Kicks, trainingInfo);
            LoadExerciseIntoMatrix("Supine First Position Kicks", "", Properties.Resources.Supine_First_Position_Kicks, trainingInfo);
            trainingInfo.mustSwitchSides = false;
            LoadExerciseIntoMatrix("Horse Stance Knee Drops", "", Properties.Resources.Horse_Stance_Knee_Drops, trainingInfo);
            // Open Front Split
            trainingInfo.splitKind = SplitKind.OpenFront;
            trainingInfo.difficulty = Difficulty.Beginner;
            trainingInfo.mustSwitchSides = true;
            LoadExerciseIntoMatrix("Side Leg Circles", "", Properties.Resources.Side_Leg_Circles2, trainingInfo);
            LoadExerciseIntoMatrix("Supine Leg Circles", "", Properties.Resources.Supine_Leg_Circles, trainingInfo);
            LoadExerciseIntoMatrix("OFS Supine Scissors", "", Properties.Resources.Supine_Scissors, trainingInfo);
            LoadExerciseIntoMatrix("OFS Prone Scissors", "", Properties.Resources.Prone_Scissors, trainingInfo);
            trainingInfo.difficulty = Difficulty.Intermediate;
            LoadExerciseIntoMatrix("Open Front Split Rocks (straight)", "", Properties.Resources.OFS_Rocks_1_straight_leg, trainingInfo);
            LoadExerciseIntoMatrix("Triangle Leg Lifts", "", Properties.Resources.Triangle_Leg_Lift, trainingInfo);
            LoadExerciseIntoMatrix("Open Front Split Rocks", "", Properties.Resources.Open_Front_Split_Rocks, trainingInfo);
            LoadExerciseIntoMatrix("OFS Prone Scissors and Pushup", "", Properties.Resources.Prone_Scissors_and_Pushup, trainingInfo);
            LoadExerciseIntoMatrix("OFS Air Splits", "", Properties.Resources.OFS_Air_Splits, trainingInfo);
            trainingInfo.difficulty = Difficulty.Advanced;            
            LoadExerciseIntoMatrix("Side Knee-Leg Lifts", "", Properties.Resources.Side_Knee_Leg_Lifts, trainingInfo);
            LoadExerciseIntoMatrix("Front Wall Warrior", "", Properties.Resources.Front_Wall_Warrior, trainingInfo);
            LoadExerciseIntoMatrix("Back Wall Warrior", "", Properties.Resources.Back_Wall_Warrior, trainingInfo);
            LoadExerciseIntoMatrix("ZejaX OFS Jumps", "", Properties.Resources.ZejaX_OFS_Jumps, trainingInfo);
            // True Front Split
            trainingInfo.splitKind = SplitKind.TrueFront;
            trainingInfo.difficulty = Difficulty.Beginner;
            LoadExerciseIntoMatrix("Crescent Kicks", "", Properties.Resources.Crescent_Kicks, trainingInfo);
            LoadExerciseIntoMatrix("Leg Lifts", "", Properties.Resources.Leg_Lifts, trainingInfo);
            LoadExerciseIntoMatrix("Scorpion Tails", "", Properties.Resources.Scorpion_Tails, trainingInfo);
            LoadExerciseIntoMatrix("Side Switch Scissors", "", Properties.Resources.Side_Switch_Scissors, trainingInfo);
            trainingInfo.difficulty = Difficulty.Intermediate;
            trainingInfo.mustSwitchSides = false;
            LoadExerciseIntoMatrix("Inverted Scissors", "", Properties.Resources.Inverted_Scissors, trainingInfo);
            trainingInfo.mustSwitchSides = true;
            LoadExerciseIntoMatrix("Downward Facing Dog Splits", "", Properties.Resources.Downward_Facing_Dog_Splits, trainingInfo);
            LoadExerciseIntoMatrix("Switch TFS Rolls", "", Properties.Resources.Switch_TFS_Rolls, trainingInfo);
            LoadExerciseIntoMatrix("True Front Split Rocks", "", Properties.Resources.True_Front_Split_Rocks, trainingInfo);
            trainingInfo.difficulty = Difficulty.Advanced;
            LoadExerciseIntoMatrix("Bend Over Splits", "", Properties.Resources.Bend_Over_Splits, trainingInfo);
            LoadExerciseIntoMatrix("TFS Rocks leg straight", "", Properties.Resources.TFS_Rocks_leg_straight, trainingInfo);
            LoadExerciseIntoMatrix("TFS Air Splits", "", Properties.Resources.TFS_Air_Splits, trainingInfo);
            trainingInfo.mustSwitchSides = false;
            LoadExerciseIntoMatrix("Switch Hamstrings Rolls", "", Properties.Resources.Switch_Hamstrings_Rolls, trainingInfo);
            trainingInfo.mustSwitchSides = true;
            LoadExerciseIntoMatrix("Celing Kicks", "", Properties.Resources.Ceiling_Kicks, trainingInfo);
            LoadExerciseIntoMatrix("Wall Lunge", "", Properties.Resources.Wall_Lunge, trainingInfo);
            LoadExerciseIntoMatrix("ZejaX TFS Jumps", "", Properties.Resources.Zejax_True_Front_Split_Jumps, trainingInfo);
            //---------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // STRENGTH \\
            trainingInfo.category = Category.Strength;
            trainingInfo.difficulty = Difficulty.All;
            trainingInfo.sets = 1;
            trainingInfo.reps = 25;
            //Side Split
            trainingInfo.splitKind = SplitKind.Side;
            trainingInfo.difficulty = Difficulty.Beginner;
            trainingInfo.mustSwitchSides = false;
            LoadExerciseIntoMatrix("Cossack Squats", "", Properties.Resources.Cossack_Squats, trainingInfo);
            trainingInfo.mustSwitchSides = true;
            LoadExerciseIntoMatrix("Front Stance Diagonal Lunges", "", Properties.Resources.Front_Stance_Diagonal_Lunges, trainingInfo);
            trainingInfo.mustSwitchSides = false;
            LoadExerciseIntoMatrix("Supine Adductors Fly", "", Properties.Resources.Supine_Adductors_Fly, trainingInfo);
            LoadExerciseIntoMatrix("Wide Squat - Wide Stance", "", Properties.Resources.Wide_Squat_and_Wide_Stance, trainingInfo);
            trainingInfo.difficulty = Difficulty.Intermediate;
            LoadExerciseIntoMatrix("Prone Flies", "", Properties.Resources.Prone_Flies, trainingInfo);
            LoadExerciseIntoMatrix("Prone Frog Pulls", "", Properties.Resources.Prone_Frog_Pulls, trainingInfo);
            trainingInfo.difficulty = Difficulty.Advanced;
            LoadExerciseIntoMatrix("Prone Frog Pulls Extending Legs", "", Properties.Resources.Prone_Frog_Pulls_Extending_Legs, trainingInfo);
            LoadExerciseIntoMatrix("Prone Frog Pulls Straight Legs", "", Properties.Resources.Prone_Frog_Pulls_Straight_Legs_to_the_Side, trainingInfo);
            LoadExerciseIntoMatrix("Straddle Leg Lifts", "", Properties.Resources.Straddle_Leg_Lifts, trainingInfo);
            LoadExerciseIntoMatrix("Supine Aductors Fly (extended)", "", Properties.Resources.Supine_Adductors_Fly__Straight_Legs_, trainingInfo);
            LoadExerciseIntoMatrix("Prone Flies (straight legs)", "", Properties.Resources.Prone_Flies__Straight_Legs_, trainingInfo);
            //Open Front Split
            trainingInfo.splitKind = SplitKind.OpenFront;
            trainingInfo.difficulty = Difficulty.Beginner;
            trainingInfo.mustSwitchSides = true;
            LoadExerciseIntoMatrix("Warrior Lunges", "", Properties.Resources.Warrior_Lunges, trainingInfo);
            LoadExerciseIntoMatrix("Warrior 4 Lifts", "", Properties.Resources.Warrior_4_Lifts, trainingInfo);
            LoadExerciseIntoMatrix("Extended Side Kick Lifts", "", Properties.Resources.Extended_Side_Kick_Lifts, trainingInfo);
            trainingInfo.difficulty = Difficulty.Intermediate;
            LoadExerciseIntoMatrix("Prone Split Lift", "", Properties.Resources.Prone_Split_Lifts, trainingInfo);
            LoadExerciseIntoMatrix("Supine Split Lift", "", Properties.Resources.Supine_Split_Lift, trainingInfo);
            trainingInfo.difficulty = Difficulty.Advanced;
            LoadExerciseIntoMatrix("Side Reverse Wall Bow", "", Properties.Resources.Side_Reverse_Wall_Bow, trainingInfo);
            LoadExerciseIntoMatrix("ZejaX Side Gables", "", Properties.Resources.ZejaX_Side_Gables, trainingInfo);


            //True Front Split
            trainingInfo.splitKind = SplitKind.TrueFront;
            trainingInfo.difficulty = Difficulty.Beginner;
            LoadExerciseIntoMatrix("Long Lunges", "", Properties.Resources.Long_Lunges, trainingInfo);
            LoadExerciseIntoMatrix("Ceiling Lifts", "", Properties.Resources.Ceiling_Lifts, trainingInfo);
            LoadExerciseIntoMatrix("Single Leg Stiff Dead Lifts", "", Properties.Resources.Single_Leg_Stiff_Dead_Lifts, trainingInfo);
            LoadExerciseIntoMatrix("Lunge Knee Drops", "", Properties.Resources.Lunge_Knee_Drops, trainingInfo);
            trainingInfo.difficulty = Difficulty.Intermediate;
            LoadExerciseIntoMatrix("Ceiling Lifts straight leg", "", Properties.Resources.Ceiling_Lifts_straight_leg, trainingInfo);
            LoadExerciseIntoMatrix("Dip Flying Splits", "", Properties.Resources.Dip_Flying_Splits, trainingInfo);
            LoadExerciseIntoMatrix("Kneeling Bows", "", Properties.Resources.Kneeling_Bows, trainingInfo);
            LoadExerciseIntoMatrix("SLSDL straight leg", "", Properties.Resources.SLSDL_leg_straight, trainingInfo);
            LoadExerciseIntoMatrix("Dip Lunge Kicks", "", Properties.Resources.Dip_Lunge_Kicks, trainingInfo);
            trainingInfo.difficulty = Difficulty.Advanced;
            LoadExerciseIntoMatrix("Airplane Turns In", "", Properties.Resources.Airplane_Turns_In, trainingInfo);
            LoadExerciseIntoMatrix("Dip Flying Splits straight legs", "", Properties.Resources.Dip_Flying_Splits_straight_legs, trainingInfo);
            LoadExerciseIntoMatrix("Dip Lunge Kicks straight leg", "", Properties.Resources.Dip_Lunge_Kicks_straight_leg, trainingInfo);
            LoadExerciseIntoMatrix("Reverse Wall Bow", "", Properties.Resources.Reverse_Wall_Bow, trainingInfo);
            LoadExerciseIntoMatrix("Split Plank Lifts", "", Properties.Resources.Split_Plank_Lifts, trainingInfo);
            LoadExerciseIntoMatrix("ZejaX Unilateral Gables", "", Properties.Resources.ZejaX_Unilateral_Gables, trainingInfo);
            //---------------------------------------------------------------------------------------------------------------------------------------------------------------------

            //Reciprocal Inhibition and cotract release for all three splits \\
            trainingInfo.category = Category.Strength;
            trainingInfo.sets = 1;
            trainingInfo.reps = 5;
            trainingInfo.splitKind = SplitKind.TrueFront;
            trainingInfo.difficulty = Difficulty.Intermediate;
            LoadExerciseIntoMatrix("RI TFS Front Leg", "Lift your leg for a few seconds, place it down and stretch. Go deeper with every repetition.", Properties.Resources.TFS_RI_Front_Leg, trainingInfo);
            LoadExerciseIntoMatrix("RI TFS Rear Leg", "Lift your leg for a few seconds, place it down and stretch. Go deeper with every repetition.", Properties.Resources.TFS_RI_Rear_Leg, trainingInfo);
            trainingInfo.difficulty = Difficulty.Advanced;
            LoadExerciseIntoMatrix("TFS Alternating Contract Release", "When one side contracts, the other stretches. Keep the tempo, 1 second each move. Go deeper into the split after doing 1 rep with each side.", Properties.Resources.TFS_Alternating_Contract_Release, trainingInfo);
            trainingInfo.splitKind = SplitKind.OpenFront;
            trainingInfo.difficulty = Difficulty.Intermediate;
            LoadExerciseIntoMatrix("RI OFS Front Leg", "Lift your leg for a few seconds, place it down and stretch. Go deeper with every repetition.", Properties.Resources.OFS_RI_Front_Leg, trainingInfo);
            LoadExerciseIntoMatrix("RI OFS Rear Leg", "Lift your leg for a few seconds, place it down and stretch. Go deeper with every repetition.", Properties.Resources.OFS_RI_Rear_Leg, trainingInfo);
            trainingInfo.difficulty = Difficulty.Advanced;
            LoadExerciseIntoMatrix("OFS Alternating Contract Release", "When one side contracts, the other stretches. Keep the tempo, 1 second each move. Go deeper into the split after doing 1 rep with each side.", Properties.Resources.OFS_Alternating_Contract_Release, trainingInfo);
            trainingInfo.splitKind = SplitKind.Side;
            trainingInfo.difficulty = Difficulty.Intermediate;
            LoadExerciseIntoMatrix("RI SS Half Frog", "Lift your leg for a few seconds, place it down and stretch. Go deeper with every repetition.", Properties.Resources.SP_Half_Frog_RI, trainingInfo);
            trainingInfo.mustSwitchSides = false;
            LoadExerciseIntoMatrix("RI SS Supine Adductors", "Open your legs as much as you can, push them against your hands for a moment, then open further.", Properties.Resources.SP_Supine_Adductors_RI, trainingInfo);
            trainingInfo.difficulty = Difficulty.Advanced;
            LoadExerciseIntoMatrix("RI SS Supine Adductors (legs straight)", "Open your legs as much as you can, push them against your hands for a moment, then open further.", Properties.Resources.SP_Supine_Adductors_RI_legs_straight, trainingInfo);
            LoadExerciseIntoMatrix("RI SS Flying Frog", "You can use a bench to place your chest on if you can't use your arms.", Properties.Resources.SP_RI_Flying_Frog, trainingInfo);
            LoadExerciseIntoMatrix("SS Alternating Contract Release", "When one side contracts, the other stretches. Keep the tempo, 1 second each move. Go deeper into the split after doing 1 rep with each side.", Properties.Resources.Side_Split_Contract_Release, trainingInfo);
            //---------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // COOL DOWN \\
            trainingInfo.category = Category.CoolDown;
            trainingInfo.splitKind = SplitKind.All;
            trainingInfo.sets = 2;
            trainingInfo.reps = 30;
            // Level 1
            trainingInfo.difficulty = Difficulty.Beginner;
            trainingInfo.mustSwitchSides = true;
            LoadExerciseIntoMatrix("L1 Knee to Chest", "", Properties.Resources.L01_Knee_to_Chest, trainingInfo);
            trainingInfo.mustSwitchSides = false;
            LoadExerciseIntoMatrix("L1 Hug Knees", "", Properties.Resources.L01_Hug_Knees, trainingInfo);
            trainingInfo.mustSwitchSides = true;
            LoadExerciseIntoMatrix("L1 Glutes", "", Properties.Resources.L01_Glutes, trainingInfo);
            LoadExerciseIntoMatrix("L1 Hamstrings", "", Properties.Resources.L01_Hamstrings, trainingInfo);
            trainingInfo.mustSwitchSides = false;
            LoadExerciseIntoMatrix("L1 Adductors", "", Properties.Resources.L01_Adductors, trainingInfo);
            trainingInfo.mustSwitchSides = true;
            LoadExerciseIntoMatrix("L1 Quads", "", Properties.Resources.L01_Quads, trainingInfo);
            trainingInfo.mustSwitchSides = false;
            LoadExerciseIntoMatrix("L1 Cobra", "", Properties.Resources.L01_Cobra, trainingInfo);
            LoadExerciseIntoMatrix("L1 Child Pose", "", Properties.Resources.L01_Child_Pose, trainingInfo);
            // Level 2
            trainingInfo.difficulty = Difficulty.Intermediate;
            trainingInfo.mustSwitchSides = true;
            LoadExerciseIntoMatrix("L2 Knee to Forehead", "", Properties.Resources.L02_Knee_to_Forehead, trainingInfo);
            trainingInfo.mustSwitchSides = false;
            LoadExerciseIntoMatrix("L2 Hug Knees", "", Properties.Resources.L02_Knee_Hug, trainingInfo);
            trainingInfo.mustSwitchSides = true;
            LoadExerciseIntoMatrix("L2 Glutes", "", Properties.Resources.L02_Glutes, trainingInfo);
            LoadExerciseIntoMatrix("L2 Hamstrings 1", "", Properties.Resources.L02_Hamstrings_1, trainingInfo);
            LoadExerciseIntoMatrix("L2 Hamstrings 2", "", Properties.Resources.L2_Hamstrings_2, trainingInfo);
            trainingInfo.mustSwitchSides = false;
            LoadExerciseIntoMatrix("L2 Adductors", "", Properties.Resources.L2_Adductors, trainingInfo);
            LoadExerciseIntoMatrix("L2 Straddle 1", "", Properties.Resources.L2_Straddle_1, trainingInfo);
            LoadExerciseIntoMatrix("L2 Straddle 2", "", Properties.Resources.L2_Straddle_2, trainingInfo);
            trainingInfo.mustSwitchSides = true;
            LoadExerciseIntoMatrix("L2 Lunge", "", Properties.Resources.L2_Lunge, trainingInfo);
            trainingInfo.mustSwitchSides = false;
            LoadExerciseIntoMatrix("L2 Suspended Cobra", "", Properties.Resources.L2_Suspended_Cobra, trainingInfo);
            LoadExerciseIntoMatrix("L2 Child Pose", "", Properties.Resources.L2_Child_Pose, trainingInfo);
            // Level 3
            trainingInfo.difficulty = Difficulty.Advanced;
            trainingInfo.mustSwitchSides = true;
            LoadExerciseIntoMatrix("L3 Knee to Opposite Shoulder", "", Properties.Resources.L3_Knee_to_Opposite_Shoulder, trainingInfo);
            LoadExerciseIntoMatrix("L3 Knee to Chest and Outside", "", Properties.Resources.L3_Knee_to_Shoulder_with_Abduction, trainingInfo);
            LoadExerciseIntoMatrix("L3 Spine Twist", "", Properties.Resources.L3_Spine_Twist, trainingInfo);
            LoadExerciseIntoMatrix("L3 Glutes", "", Properties.Resources.L3_Glutes, trainingInfo);
            LoadExerciseIntoMatrix("L3 Hamstrings (inward rotation and adduction)", "", Properties.Resources.L3_Hamstrings, trainingInfo);
            LoadExerciseIntoMatrix("L3 Inner Hamstrings", "", Properties.Resources.L3_Inner_Hamstrings, trainingInfo);
            trainingInfo.mustSwitchSides = false;
            LoadExerciseIntoMatrix("L3 Frog", "", Properties.Resources.L3_Frog, trainingInfo);
            trainingInfo.mustSwitchSides = true;
            LoadExerciseIntoMatrix("L3 Half Side Split", "", Properties.Resources.L3_Half_Side_Split, trainingInfo);
            trainingInfo.mustSwitchSides = false;
            LoadExerciseIntoMatrix("L3 Side Split", "", Properties.Resources.L3_Side_Split, trainingInfo);
            LoadExerciseIntoMatrix("L3 Straddle", "", Properties.Resources.L3_Straddle, trainingInfo);
            trainingInfo.mustSwitchSides = true;
            LoadExerciseIntoMatrix("L3 Quadriceps", "", Properties.Resources.L3_Quadriceps, trainingInfo);
            trainingInfo.mustSwitchSides = false;
            LoadExerciseIntoMatrix("L3 Back Bend", "", Properties.Resources.L3_Back_Bend, trainingInfo);
            LoadExerciseIntoMatrix("L3 Child Pose", "", Properties.Resources.L3_Child_Pose, trainingInfo);
            //--------------------------------------------------------------------------------------------------------------------
        }
    }
}
