using System;
using System.Collections.Generic;
using System.Text;

namespace ROMDataLib
{
    /// <summary>
    /// A class that represents data locations in a ROM.
    /// </summary>
    public class DataLocation
    {
        /// <summary>
        /// Gets or sets the name of the data location.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the start offset of the data location.
        /// </summary>
        public int StartOffset { get; set; }

        /// <summary> 
        /// Gets or sets the end offset of the data location. Not required.
        /// </summary>
        public int? EndOffset { get; set; }
        
        /// <summary>
        /// Gets or sets the description of the data location.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the category of the data location.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the data format for the data location.
        /// </summary>
        /// <value>The data format.</value>
        public string DataFormat { get; set; }
    }
}
