namespace HomeCinema.Entities
{
    /// <summary>
    ///     HomeCinema User's Role
    /// </summary>
    public class UserRole : IEntityBase
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public virtual Role Role { get; set; }
        public int ID { get; set; }
    }
}