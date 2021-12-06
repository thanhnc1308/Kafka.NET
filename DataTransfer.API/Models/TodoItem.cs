using System;

namespace DataTransfer.API.Models
{
    public class TodoItem
    {
        public long id { get; set; } = new Random().Next();
        public string name { get; set; }
        public bool is_complete { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;
    }
}
