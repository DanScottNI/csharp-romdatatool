using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ROMDataLib.Export
{
    public class TextExport : IGameExport
    {
        public void Export(ref Game game, string filename)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(game.Name);

            foreach (DataLocation data in game.DataLocations)
            {
                if (string.IsNullOrEmpty(data.Category))
                {
                    GenerateDataText(ref sb, data);
                }
            }
            int i = 1;

            foreach (string category in game.Categories)
            {
                sb.AppendLine(i.ToString() + ". " + category);
                sb.AppendLine();

                foreach (DataLocation data in game.DataLocations)
                {
                    if (data.Category == category)
                    {
                        GenerateDataText(ref sb, data);
                    }
                }

                i++;
            }

            if (game.DataFormats.Count > 0)
            {
                i = 1;
                sb.AppendLine();
                sb.AppendLine("Data Formats");
                sb.AppendLine();

                foreach (KeyValuePair<string, string> pair in game.DataFormats)
                {
                    sb.AppendLine(i.ToString() + ". " + pair.Key);
                    sb.AppendLine();
                    sb.AppendLine(pair.Value);
                    sb.AppendLine();
                    i++;
                }

            }
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.Write(sb.ToString());
                sw.Close();
            }
        }

        private void GenerateDataText(ref StringBuilder sb, DataLocation data)
        {
            string endOffset = string.Empty;
            if (data.EndOffset.HasValue)
            {
                endOffset = data.EndOffset.Value.ToString();
            }
            sb.Append(string.Format("{0} - ${1}", data.Name, PadHex(data.StartOffset)));
            if (!string.IsNullOrEmpty(endOffset))
            {
                if (data.EndOffset.Value != data.StartOffset)
                {
                    sb.AppendLine(" - $" + PadHex(data.EndOffset.Value));
                }
                else
                {
                    sb.AppendLine();
                }
            }
            else
            {
                sb.AppendLine();
            }

            sb.AppendLine();

            if (!string.IsNullOrEmpty(data.Description))
            {
                sb.AppendLine(data.Description);
            }

            if (!string.IsNullOrEmpty(data.DataFormat))
            {
                sb.AppendLine("See " + data.DataFormat);
            }

        }

        private string PadHex(int number)
        {
            string hexvalue = Convert.ToString(number, 16);

            if (hexvalue.Length == 1)
            {
                hexvalue = "0" + hexvalue;
            }

            return hexvalue.ToUpper();
        }
    }
}
