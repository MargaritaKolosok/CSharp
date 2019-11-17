namespace Common.Enums
{
    /// <summary>
    /// Tenant titles with their id's in UD db (see table Tenant) as values. 
    /// <para>CAUTION! Tenant title CAN NOT syntactically include another tenant 
    /// title.</para>
    /// </summary>
    public enum TenantTitle
    {
        nolang = 1,
        onelang = 2,
        manylang = 3,
        porsche9699001 = 5,
        reportitems = 6,
        import = 7,
        permissions = 8,
        emails = 9,
        china = 10,
        porsche9520000 = 11,
        Root = 12,
        Papa = 13,
        Mama = 14,
        Child_1 = 15,
        Child_2 = 16,
        Distribution1 = 17,
        DistributionEN_DE = 18,
        DistributionDE_EN = 19,
        DistributionEN1 = 20,
        DistributionAR = 21,
        reordering1 = 22,
        reordering2 = 23,
        upload1 = 24,
        upload2 = 25,
        media1 = 26,
        media2 = 27,
        advfilter = 28,
        counter = 29,
        export1 = 31,
        import1 = 32,
        All = 255
    }

    /// <summary>
    /// Tenant code must have a value that meets corresponding tenant ID in User
    /// Directory database (see table Tenant). If there is multiple tenant codes
    /// for the same tenant ID, then main tenant code must always come first. To
    /// get a string value of an additional tenant code, use "f" parameter. 
    /// <para>Example: <example>var code = TenantCode.porsche7020000.ToString("f");
    /// </example></para> 
    /// <para>CAUTION! Tenant code CAN NOT syntactically include another tenant
    /// code.</para>
    /// </summary>
    public enum TenantCode
    {
        nolang = 1,
        onelang = 2,
        manylang = 3,
        porsche9699001 = 5,
        reportitems = 6,
        import = 7,
        permissions = 8,
        emails = 9,
        porsche7000000 = 10,
        porsche7020000 = 10,
        porsche9520000 = 11,
        root = 12,
        papa = 13,
        mama = 14,
        child1 = 15,
        child2 = 16,
        distribution1 = 17,
        distributionende = 18,
        distributiondeen = 19,
        distributionen1 = 20,
        distributionar = 21,
        reordering1 = 22,
        reordering2 = 23,
        upload1 = 24,
        upload2 = 25,
        media1 = 26,
        media2 = 27,
        advfilter = 28,
        counter = 29,
        export1 = 31,
        import1 = 32,
        All = 255
    }
}
