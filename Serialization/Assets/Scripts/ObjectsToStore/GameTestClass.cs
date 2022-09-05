using System;

namespace GameTestClass
{
    [Serializable]
    public enum PlayMode// : byte
    {
        Online = 1,
        Offline = 2
    }

    public enum SequenceType
    {
        FullyRND,
        NoRepeatRND,
        InSequence
    }

    public enum YesNo : byte
    {
        Yes,
        No
    }

    public enum DefaultTopList : byte
    {
        Global,
        Local
    }
}
