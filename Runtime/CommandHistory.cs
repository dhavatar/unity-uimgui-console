namespace UImGuiConsole
{
    public class CommandHistory
    {
        private uint record;
        private uint maxRecord;
        private string[] history;

        public uint Size => record < maxRecord ? record : maxRecord;

        public uint Capacity => maxRecord;

        public string this[int index] => history[index];

        public CommandHistory(uint maxRecords = 100)
        {
            record = 0;
            maxRecord = maxRecords;
            history = new string[maxRecord];
        }

        public void PushBack(string line)
        {
            history[record++ % maxRecord] = line;
        }

        public uint GetNewIndex()
        {
            return (record - 1) % maxRecord;
        }

        public string GetNew()
        {
            return history[GetNewIndex()];
        }

        public uint GetOldIndex()
        {
            if (record <= maxRecord)
            {
                return 0;
            }
            else
            {
                return record % maxRecord;
            }
        }

        public string GetOld()
        {
            return history[GetOldIndex()];
        }

        public void Clear()
        {
            record = 0;
        }
    }
}