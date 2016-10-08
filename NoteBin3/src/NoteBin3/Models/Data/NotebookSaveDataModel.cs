namespace NoteBin3.Models.Data
{
    /// <summary>
    /// Represents data to save to the database containing notebook contents.
    /// In the future this should be done using a delta algorithm
    /// </summary>
    public class NotebookSaveDataModel
    {
        public string Contents { get; set; }
        /// <summary>
        /// Epoch time stamp
        /// </summary>
        public int TimeStamp { get; set; }
    }
}