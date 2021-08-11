using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Allprimetech.Interfaces.Models
{
    public enum Operation
    {
        Added = 1,
        Updated,
        Deleted,
        ReadAll,
        ReadSingle,
        Search
    }

    public enum Source
    {
        Customer = 1,
        Group,
        Cylinder,
        Order,
        Partner,
        LockingPlan,
        Production,
        SystemAudit,
        Disc,
        GroupsInfo
    }

    public enum ProductType
    {
        Key = 1, 
        Cylinder
    }

    public enum Status
    {
        NotStarted = 1,
        OnProgress,
        Done
    }

    public enum ArticleNumber
    {
        DZ = 1,
        KZ,
        HZ,
        VHG,
        MB23,
        MB30,
        MRZ,
        EVG,
        mLock1,
        SKA,
        SKA_N,
        SKDC,
        SKI
    }

    public enum LengthOutside
    {
        TwentySeven = 27,
        ThirtyOne = 31,
        ThirtySix = 36,
        FourtyOne = 41,
        FourtySix = 46,
        FiftyOne = 51,
        FiftySix = 56,
        SixtyOne = 61
    }

    public enum LengthInside
    {
        TwentySeven = 27,
        ThirtyOne = 31,
        ThirtySix = 36,
        FourtyOne = 41,
        FourtySix = 46,
        FiftyOne = 51,
        FiftySix = 56,
        SixtyOne = 61
    }

    public enum Color
    {
        C1, C2
    }

    public enum Options
    {
        BSZ = 1,
        KZS,
        _4E,
        SEW,
        ARS,
        VDS,
        ESS,
        ZR10,
        ZR18,
        VAR,
        SOS,
        USH,
        FL,
        AKZ,
        BLIND,
        BKZ
    }

    public enum ProductionStatus
    {
        Validated = 1,
        Blocked,
        Assembled,//cylinder
        Recliamed,
        Produced //key
    }

    public enum DiscType
    {
        A = 1, B
    }

    public enum Genre
    {
        P = 1,
        N1,
        N2,
        NG
    }

    public enum UserStatus
    {
        Activated = 1,
        Deactivated
    }

    public enum OrderDetailOperations
    {
        UPDATE,
        DELETE,
        ADD
    }

    public enum ModificationStatus
    {
        MODIFIED,
        NEW
    }
}
