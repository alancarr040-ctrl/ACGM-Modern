namespace ACGM.ModernClient.Protocol;

public static class AcgmConstants
{
    public const string LegacyVersionMajor = "0.3";
    public const string UserAgent = "ACGM " + LegacyVersionMajor;

    public const string DataStartString = "<START_HERE>";
    public const string DataStopString = "<STOP_HERE>";

    public const int MsgGetTree = 100;
    public const int MsgAddVassal = 101;
    public const int MsgRemoveVassal = 102;
    public const int MsgChangePatron = 103;
    public const int MsgLogin = 104;
    public const int MsgInitNewServer = 105;
    public const int MsgUpdateCharInfo = 106;
    public const int MsgGetCharInfo = 107;
    public const int MsgChangePassword = 108;
    public const int MsgChangeServerSetup = 109;
    public const int MsgResetPassword = 110;
    public const int MsgBackupDb = 111;
    public const int MsgChangeSecLevel = 112;
    public const int MsgGetRescueSquad = 113;
    public const int MsgGetPortalList = 114;
    public const int MsgGetTradeSkillList = 115;
    public const int MsgSearch = 116;

    public const int RetOk = 800;
    public const int RetNewServer = 801;
}
