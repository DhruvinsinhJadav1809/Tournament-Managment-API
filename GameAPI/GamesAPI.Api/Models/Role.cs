namespace GamesAPI.Api.Models
{
    public class Role
    {
        public Guid Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        } = string.Empty;

        public bool IsActive
        {
            get;
            set;
        } = true;

        public DateTime CreatedAt
        {
            get;
            set;
        } = DateTime.Now;

        public ICollection<User>
            Users
        {
            get;
            set;
        }
            = new List<User>();
    }
}