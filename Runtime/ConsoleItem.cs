using System;
using System.Collections.Generic;

namespace UImGuiConsole
{
    public enum ItemType : int
    {
        Command = 0,
        Log,
        Warning,
        Error,
        Info,
        None
    }

    public struct ConsoleItem
    {
        public static ConsoleItem NONE = new ConsoleItem(ItemType.None);

        public const string Command = "> ";
        public const string Warning = "\t[WARNING]: ";
        public const string Error = "[ERROR]: ";

        public ConsoleItem(ItemType type = ItemType.Log, string data = "")
        {
            this.type = type;
            this.data = data;
            timeStamp = DateTime.Now;
        }

        public ItemType type;
        public string data;
        public DateTime timeStamp;

        public ConsoleItem Append(string str)
        {
            data += str;
            return this;
        }

        public string Get()
        {
            switch (type)
            {
                case ItemType.Command:
                    return $"{Command}{data}";
                case ItemType.Log:
                    return $"\t{data}";
                case ItemType.Warning:
                    return $"{Warning}{data}";
                case ItemType.Error:
                    return $"{Error}{data}";
                case ItemType.Info:
                    return data;
                case ItemType.None:
                default:
                    return string.Empty;
            }
        }
    }

    public class ItemLog
    {
        public List<ConsoleItem> Items { get; private set; }

        public void Clear()
        {
            Items.Clear();
        }

        public ItemLog Log(ItemType type)
        {
            Items.Add(new ConsoleItem(type));
            return this;
        }

        public ItemLog Append(string data)
        {
            Items[^1].Append(data);
            return this;
        }
    }
}