 using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PowerAppsCMS.Constants
{
    /// <summary>
    /// Merupakan sebuah enumarasi untuk menandai status pada process
    /// </summary>
    public enum ProcessStatus
    {
        NotStarted = 0,
        OnProcess = 1,
        ONProcessLate = 2,
        StopByOperator = 3,
        WaitForQC = 4,
        QCPassed = 5,
        QCNotGood = 6,
        Finish = 7,
        Issue = 8
    }

    /// <summary>
    /// Merupakan sebuah enumarasi untuk menandai status pada process activity
    /// </summary>
    public enum ProcessActivityStatus
    {
        NotStarted = 0,
        Start = 1,
        Pause = 2,
        Break = 3,
        Stop = 4,
        StopByGroupLeader = 5
    }

    /// <summary>
    /// Merupakan sebuah enumarasi untuk menandai status pada process assign
    /// </summary>
    public enum ProcessAssignStatus
    {
        NotAssign = -1,
        NotStarted = 0,
        Start = 1,
        Pause = 2,
        Break = 3,
        Stop = 4,
        StopByGroupLeader = 5
    }

    /// <summary>
    /// Merupakan sebuah enumarasi untuk menandai type pada process assign
    /// </summary>
    public enum ProcessAssignType
    {
        Standard = 0,
        Repair = 1,
        Rework = 2
    }

    /// <summary>
    /// Merupakan sebuah enumarasi untuk menandai status pada memo
    /// </summary>
    public enum MemoStatus
    {
        NotStarted = 0,
        OnProcess = 1,
        Complete = 2
    }

    /// <summary>
    /// Merupakan sebuah enumarasi untuk menandai status pada unit
    /// </summary>
    public enum UnitStatus
    {
        NotStarted = 0,
        OnProcess = 1,
        Issue = 2,
        QCNotGood = 3
    }

    /// <summary>
    /// Merupakan sebuah enumarasi untuk menandai tipe status
    /// </summary>
    public enum TypeStatus
    {
        Standard = 0,
        Repair = 1,
        Rework = 2,
        Modification = 3
    }
}