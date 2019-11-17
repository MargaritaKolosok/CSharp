namespace Models.Interfaces
{
    /// <summary>
    /// Dummy interface that exists only for joining the CX Manager entities who should be approved on upload/import
    /// (app and item) into a single abstract class for validation purposes.
    /// <para>See also: <seealso cref="Apps.AppResponse"/>,
    /// <seealso cref="Apps.AppRequest"/>, <seealso cref="Items.Item"/></para>
    /// </summary>
    public interface IApprovalEntity
    { }
}