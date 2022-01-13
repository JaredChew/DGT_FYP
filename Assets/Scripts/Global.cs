public class Global {

    //System variables
    public static string saveFileName = "/save.giri";

    //Managers
    public static GameManager gameManager;
    public static AudioManager audioManager;

    //Total amount of X
    public static int maxItemAmount = 8;

    public static int maxControllers = 4;

    public static int maxPlayersOverall = 4;
    public static int maxPlayersInSurvival = 4;
    public static int maxPlayersInMultiplayer = 4;
    public static int maxPlayersInSingleplayer = 1;

    public static int maxCheckPointInSinglePlayer = 5;
    public static int maxDialogInSinglePlayer = 5;

    public static int minPlayersInSurvival = 1;
    public static int minPlayersInMultiplayer = 2;

    //Player Animation variables
    public static string playerAnimatorVariable_Dodge = "Dodge";
    public static string playerAnimatorVariable_Parry = "Parry";
    public static string playerAnimatorVariable_Attack = "Attack";
    public static string playerAnimatorVariable_LockOn = "Lock On";
    public static string playerAnimatorVariable_Defend = "Blocking";
    public static string playerAnimatorVariable_Parried = "Parried";
    public static string playerAnimatorVariable_Velocity = "Velocity";
    public static string playerAnimatorVariable_OnGround = "On Ground";
    public static string playerAnimatorVariable_Death = "Trigger Death";

    //Enemy Animation variables
    public static string enemyAnimatorVariable_Death = "Death";
    public static string enemyAnimatorVariable_Attack = "Attack";
    public static string enemyAnimatorVariable_Aimming = "Aimming";
    public static string enemyAnimatorVariable_Velocity = "Velocity";

    //Boss Animation variables
    public static string bossAnimatorVariable_Dodge = "Dodge";
    public static string bossAnimatorVariable_Parry = "Parry";
    public static string bossAnimatorVariable_Attack = "Attack"; 
    public static string bossAnimatorVariable_Defend = "Blocking";
    public static string bossAnimatorVariable_Parried = "Parried";
    public static string bossAnimatorVariable_Velocity = "Velocity";
    public static string bossAnimatorVariable_Kneeling = "Kneeling";
    public static string bossAnimatorVariable_OnGround = "On Ground"; 
    public static string bossAnimatorVariable_Death = "Trigger Death";
    public static string bossAnimatorVariable_DashAttack = "Dash Attack";

    //Item Animation variables
    public static string itemAnimationVariable_FireBomb = "Explode";

    //Input
    public static string inputAxes_Pause = "Pause";
    public static string inputAxes_Jump = "Jump_P";
    public static string inputAxes_Dodge = "Dodge_P";
    public static string inputAxes_Parry = "Parry_P";
    public static string inputAxes_Attack = "Attack_P";
    public static string inputAxes_Defend = "Defend_P";
    public static string inputAxes_LockOn = "Lock On_P";
    public static string inputAxes_UseItem = "Use Items_P";
    public static string inputAxes_Horizontal = "Horizontal_P";
    public static string inputAxes_SwitchItem = "Switch Items_P";
    public static string inputAxes_SelectDecline = "Select/Decline_P";

    //Tags
    public static string gameObjectTag_Tile = "Tile";
    public static string gameObjectTag_Item = "Item";
    public static string gameObjectTag_Arrow = "Arrow";
    public static string gameObjectTag_Kunai = "Kunai";
    public static string gameObjectTag_Enemy = "Enemy";
    public static string gameObjectTag_Weapon = "Weapon";
    public static string gameObjectTag_Player = "Player";
    public static string gameObjectTag_Caltrop = "Caltrop";
    public static string gameObjectTag_Platform = "Platform";
    public static string gameObjectTag_Shuriken = "Shuriken";
    public static string gameObjectTag_FireBomb = "Fire Bomb";
    public static string gameObjectTag_Parry = "Weapon Parrying";
    public static string gameObjectTag_MainCamera = "MainCamera";
    public static string gameObjectTag_WeaponMini = "Mini Weapon";
    public static string gameObjectTag_FootSpikes = "Foot Spikes";
    public static string gameObjectTag_HeavyAttack = "Heavy Attack";
    public static string gameObjectTag_PlayerSmoked = "Player Smoked";
    public static string gameObjectTag_GameController = "GameController";

    //Music - BGM-like audio
    public static string music_Test = "Test";

    //Sound - SFX-like aduio
    public static string sound_test = "Test";

    //System
    public enum windowResolution { win640x480, win800x600, win1280x720};
    //public enum graphicsQuality { veryLow, low, medium, high, veryHigh, ultra }; //Please remove if not using

    //Game level and mode
    public enum gameModes { notInGameMode, singlePlayer, multiPlayer, survival };
    public enum sceneMenuIndex { mainMenu, multiPlayer_L1, multiPlayer_L2, multiPlayer_L3, multiPlayer_L4, multiPlayer_L5, singlePlayer, survival, nullScene };

    //Game Objects
    public enum typeOfEnemy { bat, ghost, skeleton, skeletonArcher, skeletonBrute, skeletonShield, samurai, none }
    public enum items { itemNone, kunai, caltrop, fireBomb, elixer, shuriken, phaseWalk, smokeBomb, footSpikes, grapplingHook };
    public enum bossItem { kunai, fireBomb, shuriken };

    //Singleplayer
    public enum spScenario { event1, event2, event3, event4, event5, event6 };
    public enum spArea { area1, area2, area3, area4, area5, area6, area7, a };
    public enum spCheckpointArea { area1, area2, area4A, area4B, area6, a };
    public enum spPortals { portalA, portalB, portalC, portalD, portalE, portalF, portalG, portalH, portalI, portalJ };
    public enum spAreaTransition { transition1, transition2, transition3, transition4, transition5, transition6A, transition6B };
    public enum spEnemySpawnLocation { area1_A, area1_B, area2_A, area2_B, area2_C, area2_D, area2_E, area4_A, area4_B, area4_C, area4_D, area4_E, area4_F, area4_G, area4_H, area4_I, area5_A, area5_B, area5_C, area5_D, area5_E, area5_F, area5_G, area5_H, area7_A };

    //Survival
    public enum svSpawnLocation { spawnA, spawnB, spawnC, spawnD, spawnE };

}