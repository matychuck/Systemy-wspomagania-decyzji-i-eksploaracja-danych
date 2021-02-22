using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SWD
{
    public class Tree
    {
        public TreeNode Root { get; set; }
        static public List<List<string>> rules = new List<List<string>>();
        static private string decisionClass = "";

        public static List<List<string>> Print(TreeNode node, string result)
        {
            if (node?.ChildNodes == null || node.ChildNodes.Count == 0)
            {
                var seperatedResult = result.Split(' ');
                rules.Add(new List<string>());
                foreach (var item in seperatedResult)
                {
                    rules[rules.Count - 1].Add($"{item} ");
                }

                return rules;
            }

            foreach (var child in node.ChildNodes)
            {
                Print(child, result + " -- " + child.Edge.ToLower() + " --> " + child.Name.ToUpper());
            }

            return rules;
        }

        public static string CalculateResult(TreeNode root, IDictionary<string, string> valuesForQuery, string result)
        {
            var valueFound = false;

            result += root.Name.ToUpper() + " -- ";

            if (root.IsLeaf)
            {
                result = root.Edge.ToLower() + " --> " + root.Name.ToUpper();
                valueFound = true;
            }
            else
            {
                foreach (var childNode in root.ChildNodes)
                {
                    foreach (var entry in valuesForQuery)
                    {
                        if (childNode.Edge.ToUpper().Equals(entry.Value.ToUpper()) && root.Name.ToUpper().Equals(entry.Key.ToUpper()))
                        {
                            valuesForQuery.Remove(entry.Key);

                            return result + CalculateResult(childNode, valuesForQuery, $"{childNode.Edge.ToLower()} --> ");
                        }
                    }
                }
            }

            // if the user entered an invalid attribute
            if (!valueFound)
            {
                result = "Attribute not found";
            }

            return result;
        }

        public static TreeNode Learn(DataTable data, string edgeName)
        {
            var root = GetRootNode(data, edgeName);

            foreach (var item in root.NodeAttribute.DifferentAttributeNames)
            {
                // if a leaf, leaf will be added in this method
                var isLeaf = CheckIfIsLeaf(root, data, item);

                // make a recursive call as long as the node is not a leaf
                if (!isLeaf)
                {
                    var reducedTable = CreateSmallerTable(data, item, root.TableIndex);

                    root.ChildNodes.Add(Learn(reducedTable, item));
                }
            }

            return root;
        }

        private static bool CheckIfIsLeaf(TreeNode root, DataTable data, string attributeToCheck)
        {
            string max = "";
            var chooseMajority = false;
            var isLeaf = true;
            var allEndValues = new List<string>();
            Dictionary<string, int> classes = new Dictionary<string, int>();

            // get all leaf values for the attribute in question
            for (var i = 0; i < data.Rows.Count; i++)
            {
                if (data.Rows[i][root.TableIndex].ToString().Equals(attributeToCheck))
                {
                    allEndValues.Add(data.Rows[i][data.Columns.Count - 1].ToString());
                    if (!classes.ContainsKey(data.Rows[i][data.Columns.Count - 1].ToString()))
                    {
                        classes.Add(data.Rows[i][data.Columns.Count - 1].ToString(), 1);
                    }
                    else
                    {
                        classes[data.Rows[i][data.Columns.Count - 1].ToString()] = classes[data.Rows[i][data.Columns.Count - 1].ToString()] + 1;
                    }
                }
            }

            // check whether all elements of the list have the same value
            if (allEndValues.Count > 0 && allEndValues.Any(x => x != allEndValues[0]))
            {
                isLeaf = false;
            }

            if (!isLeaf && data.Columns.Count == 2)
            {
                isLeaf = true;
                chooseMajority = true;
                max = classes.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
            }

            // create leaf with value to display and edge to the leaf
            if (isLeaf)
            {
                if (chooseMajority)
                    root.ChildNodes.Add(new TreeNode(true, max, attributeToCheck));
                else
                    root.ChildNodes.Add(new TreeNode(true, allEndValues[0], attributeToCheck));
            }

            return isLeaf;
        }

        private static DataTable CreateSmallerTable(DataTable data, string edgePointingToNextNode, int rootTableIndex)
        {
            var smallerData = new DataTable();

            // add column titles
            for (var i = 0; i < data.Columns.Count; i++)
            {
                smallerData.Columns.Add(data.Columns[i].ToString());
            }

            // add rows which contain edgePointingToNextNode to new datatable
            for (var i = 0; i < data.Rows.Count; i++)
            {
                if (data.Rows[i][rootTableIndex].ToString().Equals(edgePointingToNextNode))
                {
                    var row = new string[data.Columns.Count];

                    for (var j = 0; j < data.Columns.Count; j++)
                    {
                        row[j] = data.Rows[i][j].ToString();
                    }

                    smallerData.Rows.Add(row);
                }
            }

            // remove column which was already used as node            
            smallerData.Columns.Remove(smallerData.Columns[rootTableIndex]);

            return smallerData;
        }

        private static TreeNode GetRootNode(DataTable data, string edge)
        {
            var attributes = new List<MyAttribute>();
            var highestInformationGainIndex = -1;
            var highestInformationGain = double.MinValue;

            // Get all names, amount of attributes and attributes for every column             
            for (var i = 0; i < data.Columns.Count - 1; i++)
            {
                var differentAttributenames = MyAttribute.GetDifferentAttributeNamesOfColumn(data, i);
                attributes.Add(new MyAttribute(data.Columns[i].ToString(), differentAttributenames));
            }

            // Calculate Entropy (S)
            var tableEntropy = CalculateTableEntropy(data);

            for (var i = 0; i < attributes.Count; i++)
            {
                attributes[i].InformationGain = GetGainForAllAttributes(data, i, tableEntropy);

                if (attributes[i].InformationGain > highestInformationGain)
                {
                    highestInformationGain = attributes[i].InformationGain;
                    highestInformationGainIndex = i;
                }
            }

            try
            {
                return new TreeNode(attributes[highestInformationGainIndex].Name, highestInformationGainIndex, attributes[highestInformationGainIndex], edge);
            }catch(Exception e)
            {
                return null;
            }
        }

        private static double GetGainForAllAttributes(DataTable data, int colIndex, double entropyOfDataset)
        {
            var totalRows = data.Rows.Count;
            var amountForDifferentValue = GetAmountOfEdgesAndTotalPositivResults(data, colIndex);
            var stepsForCalculation = new List<double>();

            foreach (var item in amountForDifferentValue)
            {
                // helper for calculation
                var firstDivision = item[0, 1] / (double)item[0, 0];
                var secondDivision = (item[0, 0] - item[0, 1]) / (double)item[0, 0];

                // prevent dividedByZeroException
                if (firstDivision == 0 || secondDivision == 0)
                {
                    stepsForCalculation.Add(0.0);
                }
                else
                {
                    stepsForCalculation.Add(-firstDivision * Math.Log(firstDivision, 2) - secondDivision * Math.Log(secondDivision, 2));
                }
            }

            var gain = stepsForCalculation.Select((t, i) => amountForDifferentValue[i][0, 0] / (double)totalRows * t).Sum();

            gain = entropyOfDataset - gain;

            return gain;
        }

        private static double CalculateTableEntropy(DataTable data)
        {
            var totalRows = data.Rows.Count;
            var amountForDifferentValue = GetAmountOfEdgesAndTotalPositivResults(data, data.Columns.Count - 1);

            var stepsForCalculation = amountForDifferentValue
                .Select(item => item[0, 0] / (double)totalRows)
                .Select(division => -division * Math.Log(division, 2))
                .ToList();

            return stepsForCalculation.Sum();
        }

        private static List<int[,]> GetAmountOfEdgesAndTotalPositivResults(DataTable data, int indexOfColumnToCheck)
        {
            var foundValues = new List<int[,]>();
            var knownValues = CountKnownValues(data, indexOfColumnToCheck);

            foreach (var item in knownValues)
            {
                var amount = 0;
                var positiveAmount = 0;

                for (var i = 0; i < data.Rows.Count; i++)
                {
                    if (data.Rows[i][indexOfColumnToCheck].ToString().Equals(item))
                    {
                        amount++;

                        // Counts the positive cases and adds the sum later to the array for the calculation
                        if (data.Rows[i][data.Columns.Count - 1].ToString().Equals(data.Rows[0][data.Columns.Count - 1]))
                        {
                            positiveAmount++;
                        }
                    }
                }

                int[,] array = { { amount, positiveAmount } };
                foundValues.Add(array);
            }

            return foundValues;
        }

        private static IEnumerable<string> CountKnownValues(DataTable data, int indexOfColumnToCheck)
        {
            var knownValues = new List<string>();

            // add the value of the first row to the list
            if (data.Rows.Count > 0)
            {
                knownValues.Add(data.Rows[0][indexOfColumnToCheck].ToString());
            }

            for (var j = 1; j < data.Rows.Count; j++)
            {
                var newValue = knownValues.All(item => !data.Rows[j][indexOfColumnToCheck].ToString().Equals(item));

                if (newValue)
                {
                    knownValues.Add(data.Rows[j][indexOfColumnToCheck].ToString());
                }
            }

            return knownValues;
        }

        public static string DecideResultClass(TreeNode root, Dictionary<string,string> element, int index = 0)
        {
            if (root?.ChildNodes != null)
            {
                if (root.ChildNodes.Count != 0)
                {
                    foreach (var item in root.ChildNodes)
                    {
                        if (element[root.Name] == item.Edge)
                        {
                            DecideResultClass(item, element, index++);
                        }
                    }
                }
            }
            else
            {
                decisionClass = root.Name;
            }

            return decisionClass;
        }

        public void ClearRules()
        {
            rules.Clear();
        }
    }
}
