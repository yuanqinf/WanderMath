public static class Constants
{
    public const float ROTATION_DEGREE = 4.5f;
    public const float ROTATION_THRESHOLD = 20f;
    public const float GENERIC_SUBTITLE_DELAY = 1.0f;
    public const float BIGWIN_ANIMATION_DELAY = 3.0f;
    public const float HALF_FEET = 0.15f;
    public const float ONE_FEET = 0.30f;
    public const float FIVE_FEET = 1.50f;
    public const float MOVEMENT_RANGE = 0.008f;
    public const float FillAmountVolFor2 = 2;
    public const float FillAmountVolFor4 = 4;
    public struct Tags {
        public const string CUBE_EASY = "CUBE_EASY";
        public const string DestroyCollider = "destroyCollider";
        public const string DOT = "dot";
        public const string Finley = "finley";
        public const string Balloon = "balloon";
        public const string ShootBtn = "shoot_btn";
        public const string Cannon = "cannon";
        public const string Muzzle = "muzzle";
    }

    public struct Animation
    {
        public const string IsShotTrigger = "isShot";
        public const string IsShootingTrigger = "isShooting";
        public const string DestroyCollider = "destroyCollider";
        public const string DOT = "dot";
        public const string Finley = "finley";
        public const string Balloon = "balloon";
    }

    public struct Objects
    {
        public const string RailingStartPoint = "RailingStartPoint";
        public const string RailingEndPoint = "RailingEndPoint";
        public const string RampStartPoint = "RampStartPoint";
        public const string RampEndPoint = "RampEndPoint";
    }

    public struct VoiceOvers
    {
        public const string PHASE0Start = "phase0Start";
        public const string PHASE0End = "phase0End";
        public const string PHASE1Start = "phase1Start";
        public const string PHASE1Mid = "phase1Mid";
        public const string PHASE1End = "phase1End";
        public const string PHASE2Start = "phase2Start";
        public const string PHASE2Mid = "phase2Mid";
        public const string PHASE2End = "phase2End";
        public const string PHASE3Start = "phase3Start";
        public const string PHASE3End = "phase3End";
        public const string PHASE3Additional = "phase3Additional";
    }

    public struct GamePhase
    {
        public const string PHASE0 = "phase0";
        public const string PHASE1 = "phase1";
        public const string PHASE2 = "phase2";
        public const string PHASE3 = "phase3";
        public const string WAITING = "waiting";
        public const string ENDING = "ending";
        public const string SETUP = "setup";
    }

    public struct ShapeNames
    {
        public const string CUBOID = "CUBOID";
        public const string HEXAGON = "HEXAGON";
        public const string PYRAMID = "PYRAMID";
        public const string CUBE_EASY = "CUBE_EASY";
        public const string CUBE_MED = "CUBE_MED";
        public const string CUBE_MED2 = "CUBE_MED2";
        public const string CUBE_WRONG = "CUBE_WRONG";
    }
}
