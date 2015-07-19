using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ROMDataLib
{
    /// <summary>
    /// Manages ROM data.
    /// </summary>
    public class ROMDataManager
    {
        /// <summary>
        /// Saves the ROM data.
        /// </summary>
        /// <param name="game">The game object.</param>
        /// <param name="fileName">Name of the file.</param>
        public void SaveROMData(ref Game game, string fileName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", null, null));

            // First, we have to create the <game> node.
            XmlElement gameNode = xmlDoc.CreateElement("game");

            // Create the name element within the game element.
            XmlElement nameNode = xmlDoc.CreateElement("name");
            nameNode.InnerText = game.Name;
            gameNode.AppendChild(nameNode);

            // Create the size element within the game element.
            XmlElement sizeNode = xmlDoc.CreateElement("size");
            sizeNode.InnerText = Convert.ToString(game.Size, 16);
            gameNode.AppendChild(sizeNode);

            if (game.Categories != null && game.Categories.Count > 0)
            {
                XmlElement categoriesNode = xmlDoc.CreateElement("categories");
                gameNode.AppendChild(categoriesNode);

                foreach (string category in game.Categories)
                {
                    XmlElement categoryNode = xmlDoc.CreateElement("category");
                    categoryNode.InnerText = category;
                    categoriesNode.AppendChild(categoryNode);
                }
            }

            if (game.DataFormats != null & game.DataFormats.Count > 0)
            {
                XmlElement formatsNode = xmlDoc.CreateElement("dataformats");
                gameNode.AppendChild(formatsNode);

                foreach (KeyValuePair<string,string> pair in game.DataFormats)
                {
                    XmlElement dataFormatNode = xmlDoc.CreateElement("dataformat");
                    dataFormatNode.InnerText = pair.Value;

                    // Set up the attribute for saving of the name.
                    XmlAttribute dataFormatAtt = xmlDoc.CreateAttribute("name");
                    dataFormatAtt.InnerText = pair.Key;
                    dataFormatNode.Attributes.Append(dataFormatAtt);

                    formatsNode.AppendChild(dataFormatNode);
                }
            }

            if (game.DataLocations != null && game.DataLocations.Count > 0)
            {
                // Create the grouping element for the data locations.
                XmlElement locsNode = xmlDoc.CreateElement("datalocations");
                gameNode.AppendChild(locsNode);

                // Now, if there are data locations, write them out to the game node.
                foreach (DataLocation locs in game.DataLocations)
                {
                    // Create an element for this data location.
                    XmlElement locNode = xmlDoc.CreateElement("datalocation");

                    // Create an attribute for this data location, for the name.
                    XmlAttribute nameAtt = xmlDoc.CreateAttribute("name");
                    nameAtt.Value = locs.Name;
                    locNode.Attributes.Append(nameAtt);

                    // Create an attribute for this data location, for the category. If
                    // one is entered.
                    if (locs.Category != string.Empty)
                    {
                        XmlAttribute catAtt = xmlDoc.CreateAttribute("category");
                        catAtt.Value = locs.Category;
                        locNode.Attributes.Append(catAtt);
                    }

                    // Create an attribute for this data location, for the start offset.
                    // The start offset should be saved as a hexadecimal (base 16) value.
                    XmlAttribute startOffsetAtt = xmlDoc.CreateAttribute("startOffset");
                    startOffsetAtt.Value = Convert.ToString(locs.StartOffset, 16);
                    locNode.Attributes.Append(startOffsetAtt);

                    // If the end offset value exists, then create an attribute for the end
                    // offset.  The end offset should be saved as a hexadecimal (base 16) value.
                    if (locs.EndOffset != null)
                    {
                        XmlAttribute endOffsetAtt = xmlDoc.CreateAttribute("endOffset");
                        endOffsetAtt.Value = Convert.ToString(Convert.ToInt32(locs.EndOffset), 16);
                        locNode.Attributes.Append(endOffsetAtt);
                    }

                    // Create a new element for the description of the data location.
                    // This should be appended as a child, into in the <datalocation> element.
                    XmlElement descNode = xmlDoc.CreateElement("description");
                    descNode.InnerText = locs.Description;
                    locNode.AppendChild(descNode);

                    locsNode.AppendChild(locNode);
                }
            }

            xmlDoc.AppendChild(gameNode);
            xmlDoc.Save(fileName);
        }

        /// <summary>
        /// Loads the ROM data from an XML file.
        /// </summary>
        /// <param name="filename">The filename of the XML file.</param>
        /// <returns>A Game object that represents the data in the XML file.</returns>
        public Game LoadROMData(string filename)
        {
            Game game = new Game();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filename);

            foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
            {
                // Retrieve the name of the game.
                if (node.Name == "name")
                {
                    game.Name = node.InnerText;
                }

                if (node.Name == "size")
                {
                    game.Size = Convert.ToInt32(node.InnerText, 16);
                }

                if (node.Name == "categories")
                {
                    foreach (XmlNode category in node.ChildNodes)
                    {
                        game.Categories.Add(category.InnerText);
                    }
                }

                if (node.Name == "dataformats")
                {
                    foreach (XmlNode dataformat in node.ChildNodes)
                    {
                        string name = string.Empty;
                        
                        // Attempt to search for the name attribute.
                        foreach (XmlAttribute att in dataformat.Attributes)
                        {
                            if (att.Name == "name")
                            {
                                name = att.InnerText;
                            }
                        }
                        
                        game.DataFormats.Add(name, dataformat.InnerText);
                    }
                }

                if (node.Name == "datalocations")
                {
                    foreach (XmlNode datalocs in node.ChildNodes)
                    {
                        DataLocation loc = new DataLocation();
                        foreach (XmlAttribute att in datalocs.Attributes)
                        {
                            if (att.Name == "name")
                            {
                                loc.Name = att.InnerText;
                            }
                            else if (att.Name == "startOffset")
                            {
                                loc.StartOffset = Convert.ToInt32("0x" + att.InnerText, 16);
                            }
                            else if (att.Name == "endOffset")
                            {
                                loc.EndOffset = Convert.ToInt32("0x" + att.InnerText, 16);
                            }
                            else if (att.Name == "category")
                            {
                                loc.Category = att.InnerText;
                            }
                        }

                        foreach (XmlNode descnode in datalocs.ChildNodes)
                        {
                            if (descnode.Name == "description")
                            {
                                loc.Description = descnode.InnerText;
                            }
                        }

                        game.DataLocations.Add(loc);
                    }
                }
            }
            game.SortDataLocations();
            return game;
        }
    }
}
