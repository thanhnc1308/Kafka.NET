using System;

namespace Kafka.NET.Models
{
    public class TodoItem
    {
        public long id { get; set; } = new Random().Next();
        public string name { get; set; }
        public bool is_complete { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;

        public TodoItem() {
            name = "item";
            is_complete = false;
        }

        public TodoItem(string _name, bool _is_complete)
        {
            name = _name;
            is_complete = _is_complete;
        }
    }
}
