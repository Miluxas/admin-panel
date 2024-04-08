namespace AdminPanel.Data
{
	public class ToDo
	{
        public Guid Id { get; set; } = Guid.Empty;

        public Guid UserId { get; set; } = Guid.Empty;

        public string Title { get; set; } = string.Empty;

        public double Effort { get; set; }

        public string Status { get; set; } = string.Empty;

    }
}

