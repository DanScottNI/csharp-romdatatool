using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

namespace ROMDataLib
{
    /// <summary>
    /// A game that has data locations.
    /// </summary>
    public class Game
    {
        /// <summary>
        /// Gets or sets the name of the game.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the number of bytes in the ROM.
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Gets or sets the list of data locations.
        /// </summary>
        public List<DataLocation> DataLocations { get; set; }

        /// <summary>
        /// Gets or sets the categories that are available for offsets.
        /// </summary>
        public StringCollection Categories { get; set; }

        /// <summary>
        /// Gets or sets the data formats that in this game.
        /// </summary>
        public Dictionary<string, string> DataFormats { get; set; }

        public Game()
        {
            this.DataLocations = new List<DataLocation>();
            this.Categories = new StringCollection();
            this.DataFormats = new Dictionary<string, string>();
        }

        /// <summary>
        /// Loops through the data locations, searching for locations with the category
        /// </summary>
        /// <param name="oldCategory">The old category to search for.</param>
        /// <param name="newCategory">The new category to replace the old category with.</param>
        public void UpdateGameCategories(string oldCategory, string newCategory)
        {
            foreach (DataLocation datalocation in DataLocations)
            {
                if (datalocation.Category == oldCategory)
                {
                    datalocation.Category = newCategory;
                }
            }
        }

        public void SortDataLocations()
        {
            DataLocations.Sort(delegate(DataLocation p1, DataLocation p2) { return p1.StartOffset.CompareTo(p2.StartOffset); });
        }

    }
}
