using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQL2LINQ {
    public partial class SQL2LINQ : Form {
        public SQL2LINQ() {
            InitializeComponent();
        }

        public string searchTableName { get; set; }
        public string ViewModelText = "";

        private Dictionary<string, char> TableCharMapping = new Dictionary<string, char>();
        private Dictionary<string, string> TableActualNameMapping = new Dictionary<string, string>();
        private bool isInnerJoinPresent = false;
        private char characterUsed = 'a';
        private bool isOrderByPresent = false;
        private bool isWherePresent = false;
        private List<int> ListOpenParentheses = new List<int>();
        private List<int> ListCloseParentheses = new List<int>();
        private List<int> ListOpenBracket = new List<int>();
        private List<int> ListCloseBracket = new List<int>();

        private void btnSql2Linq_Click(object sender, EventArgs e) {
            characterUsed = 'a';
            try {
                string sqlText = GetSqlWithSpaces();
                bool isSelectPresent = sqlText.Contains("select ");
                bool isFromPresent = sqlText.Contains(" from ");
                if (isSelectPresent && isFromPresent) {
                    var getParts = GetPartsOfSQL(sqlText);
                    GetParenthesesInSql(getParts);
                    int count = 0;
                    int selectIndex = 0;
                    int fromIndex = 0;
                    List<int> innerjoinIndexList = new List<int>();
                    List<int> orderbyIndexList = new List<int>();
                    List<int> whereIndexList = new List<int>();
                    foreach (var gp in getParts) {
                        int innerjoinIndex = 0;
                        int orderbyIndex = 0;
                        int whereIndex = 0;
                        if (gp == "select") {
                            selectIndex = count;
                        } else if (gp == "from") {
                            fromIndex = count;
                        } else if (gp == "inner_join") {
                            innerjoinIndex = count;
                            innerjoinIndexList.Add(innerjoinIndex);
                        } else if (gp == "order_by") {
                            orderbyIndex = count;
                            orderbyIndexList.Add(orderbyIndex);
                        } else if (gp == "where") {
                            whereIndex = count;
                            whereIndexList.Add(whereIndex);
                        }
                        count++;
                    }
                    string preparedString = GetPreparedString(
                        getParts,
                        selectIndex,
                        innerjoinIndexList,
                        fromIndex,
                        orderbyIndexList,
                        whereIndexList);
                    txtLINQ.Text = preparedString;
                } else {
                    txtLINQ.Text = "";
                    MessageBox.Show("SQL needs to be a select statement");
                }
            } catch (Exception) {
                txtLINQ.Text = "";
                MessageBox.Show("SQL2LINQ was unable to convert the query. Please enter the LINQ query manually or change the queary to a simpler form.");
            }
        }
        private string GetPreparedString(string[] getParts, int selectIndex, List<int> innerjoinIndexList, int fromIndex, List<int> orderbyIndexList, List<int> whereIndexList) {
            string fromTable = getParts[fromIndex + 1];
            string JoinString = GetJoinString(getParts, innerjoinIndexList, fromTable);
            string OrderByString = GetOrderByString(getParts, orderbyIndexList);
            string WhereString = GetWhereString(getParts, whereIndexList);
            List<string> selectedColumnList = GetSelectedColumn(getParts, selectIndex).Split(',').ToList();
            string LinqSelectedColumns = GetLinqSelectedColumns(selectedColumnList, fromTable);
            string preparedString = "from " + TableCharMapping[fromTable] + " in " + Globals.UserDefinedObjectName + "." + TableActualNameMapping[fromTable]
                + ((!isInnerJoinPresent) ? "" : JoinString)
                + ((!isOrderByPresent) ? "" : OrderByString)
                + ((!isWherePresent) ? "" : WhereString)
                + Environment.NewLine + "select new " + searchTableName + "Details { "
                + LinqSelectedColumns
                + Environment.NewLine + "}";
            return preparedString;
        }

        private void GetParenthesesInSql(string[] getParts) {
            string OpenParentheses = "(", CloseParentheses = ")", OpenBracket = "{", CloseBracket = "}";
            ListOpenParentheses = GetAllOccurencesOfStringInStringArray(getParts, OpenParentheses);
            ListCloseParentheses = GetAllOccurencesOfStringInStringArray(getParts, CloseParentheses);
            ListOpenBracket = GetAllOccurencesOfStringInStringArray(getParts, OpenBracket);
            ListCloseBracket = GetAllOccurencesOfStringInStringArray(getParts, CloseBracket);
        }
        private List<int> GetAllOccurencesOfStringInStringArray(string[] sqlText, string matchString) {
            var foundIndexes = new List<int>();
            int i = -1;
            foreach (var st in sqlText) {
                i++;
                if (st == matchString) {
                    foundIndexes.Add(i);
                    continue;
                }
            }
            return foundIndexes;
        }

        private static List<int> GetAllOccurencesOfCharacterInString(string sqlText, char matchCharacter) {
            var foundIndexes = new List<int>();
            for (int i = sqlText.IndexOf(matchCharacter); i > -1; i = sqlText.IndexOf(matchCharacter, i + 1)) {
                foundIndexes.Add(i);
            }
            return foundIndexes;
        }
        private string GetWhereString(string[] getParts, List<int> whereIndexList) {
            string WhereString = "";
            int getIndex = 0;
            string condition = "";
            foreach (int index in whereIndexList) {
                getIndex = index;
                while (getParts[getIndex + 1].Equals("("))
                    getIndex++; // Handle this case properly in the future
                condition += GetConditionForParts(getParts, ref getIndex);
                getIndex++;
                string LogicalOperation = "";
                while (getParts[getIndex] == "and" || getParts[getIndex] == "or") {
                    if (getParts[getIndex] == "and") {
                        LogicalOperation = "&&";
                    } else if (getParts[getIndex] == "or") {
                        LogicalOperation = "||";
                    }
                    condition += " " + LogicalOperation + " " + GetConditionForParts(getParts, ref getIndex);
                    getIndex++;
                }
                if (getParts[getIndex] == ")")
                    break;// Handle this case properly in the future
            }
            WhereString += Environment.NewLine + "where " + condition;
            return WhereString;
        }
        private string GetConditionForParts(string[] getParts, ref int getIndex) {
            getIndex++;
            string lhs = GetLHS(getParts, getIndex);
            getIndex++;
            string logicalOperator = GetOperator(getParts, ref getIndex);
            getIndex++;
            string rhs = GetRHS(getParts, ref getIndex);
            return lhs + " " + logicalOperator + " " + rhs;
        }
        private string GetLHS(string[] getParts, int getIndex) {
            string lhs = "";
            if (getParts[getIndex].Contains(".")) {
                var splitWhere = getParts[getIndex].Split('.');
                string whereTable = splitWhere[0];
                string whereColumn = splitWhere[1];
                string ActualTableName = TableActualNameMapping[whereTable];
                string ActualColumnName = GetActualTableColumnNameFromTable(ActualTableName, whereColumn);
                char TableChar = TableCharMapping[whereTable];
                lhs = TableChar + "." + ActualColumnName;
            } else {
                lhs = getParts[getIndex];
            }

            return lhs;
        }
        private string GetOperator(string[] getParts, ref int getIndex) {
            string logicalOperator = "";
            string SecondIndex = getParts[getIndex];
            if (SecondIndex.Equals("=") || SecondIndex.Equals("==") || SecondIndex.Equals("is")) {
                logicalOperator = "==";
            } else if (SecondIndex.Equals("!=") || SecondIndex.Equals("<>")) {
                logicalOperator = "!=";
            } else if (SecondIndex.Equals("<=")) {
                logicalOperator = "<=";
            } else if (SecondIndex.Equals(">=")) {
                logicalOperator = ">=";
            } else if (SecondIndex.Equals("<")) {
                logicalOperator = "<";
            } else if (SecondIndex.Equals(">")) {
                logicalOperator = ">";
            } else if (SecondIndex.Equals("like")) {
                logicalOperator = "like";
            } else if (SecondIndex.Equals("+")) {
                logicalOperator = "+";
            } else if (SecondIndex.Equals("-")) {
                logicalOperator = "-";
            } else if (SecondIndex.Equals("*")) {
                logicalOperator = "*";
            } else if (SecondIndex.Equals("/")) {
                logicalOperator = "/";
            } else {
                logicalOperator = SecondIndex;
            }
            return logicalOperator;
        }
        private string GetRHS(string[] getParts, ref int getIndex) {
            string rhs = "";
            string ThirdIndex = getParts[getIndex];
            string FourthIndex = "";
            if (ThirdIndex == "-" || ThirdIndex == "+") {
                getIndex++;
                FourthIndex = getParts[getIndex];
            }
            string rhsString = ThirdIndex + FourthIndex;
            if (rhsString.Contains(".")) {
                var splitWhere = rhsString.Split('.');
                string whereTable = splitWhere[0];
                string whereColumn = splitWhere[1];
                string ActualTableName = TableActualNameMapping[whereTable];
                string ActualColumnName = GetActualTableColumnNameFromTable(ActualTableName, whereColumn);
                char TableChar = TableCharMapping[whereTable];
                rhs = TableChar + "." + ActualColumnName;
            } else {
                rhs = rhsString;
            }

            return rhs;
        }
        private string GetOrderByString(string[] getParts, List<int> orderbyIndexList) {
            string OrderByString = "";
            foreach (int index in orderbyIndexList) {
                var splitOrder = getParts[index + 1].Split('.');
                string orderTable = splitOrder[0];
                string orderColumn = splitOrder[1];
                string ActualTableName = TableActualNameMapping[orderTable];
                string ActualColumnName = GetActualTableColumnNameFromTable(ActualTableName, orderColumn);
                string order = "";
                if ((index + 2) < getParts.Length) {
                    if (getParts[index + 2] == "asc") {
                        order = "ascending";
                    } else if (getParts[index + 2] == "desc") {
                        order = "descending";
                    }
                }
                OrderByString += Environment.NewLine + "orderby " + ActualTableName + "." + ActualColumnName + (string.IsNullOrEmpty(order) ? "" : " " + order);
            }
            return OrderByString;
        }
        private string[] GetPartsOfSQL(string sqlText) {
            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex("[ ]{2,}", options);
            sqlText = regex.Replace(sqlText, " ");
            sqlText = sqlText.Replace(", ", ",");
            sqlText = sqlText.Replace(" ,", ",");
            isInnerJoinPresent = CheckForJoins(sqlText);
            if (isInnerJoinPresent) {
                sqlText = sqlText.Replace(" inner join ", " inner_join ");
                sqlText = sqlText.Replace(" left outer join ", " inner_join ");
                sqlText = sqlText.Replace(" left join ", " inner_join ");
            }
            isOrderByPresent = CheckForOrderBys(sqlText);
            if (isOrderByPresent) {
                sqlText = sqlText.Replace(" order by ", " order_by ");
            }
            isWherePresent = CheckForWhere(sqlText);
            return sqlText.Split(' ');
        }
        private bool CheckForWhere(string sqlText) {
            return sqlText.Contains(" where ");
        }
        private static bool CheckForJoins(string sqlText) {
            return sqlText.Contains(" inner join ") || sqlText.Contains(" left outer join ") || sqlText.Contains(" left join ");
        }
        private static bool CheckForOrderBys(string sqlText) {
            return sqlText.Contains(" order by ");
        }
        private string GetSelectedColumn(string[] getParts, int selectIndex) {
            int selectionIndex = selectIndex + 1;
            string selectedColumn = getParts[selectionIndex];
            selectionIndex++;
            while (getParts[selectionIndex] == "as" || getParts[selectionIndex] == "when" || selectedColumn.EndsWith("(")) {
                if (getParts[selectionIndex] == "as") {
                    selectionIndex++;
                    selectedColumn += "_as_" + getParts[selectionIndex];
                    selectionIndex++;
                }
                if (selectedColumn.EndsWith("case") || getParts[selectionIndex] == "when") {
                    if (selectedColumn.EndsWith("case")) {
                        selectedColumn = selectedColumn.Substring(0, selectedColumn.Length - 4);
                    }
                    string selectedLine = HandleCaseWhenThen(getParts, ref selectionIndex);
                    selectedColumn += selectedLine.Replace("'", "\"");
                }
                if (selectedColumn.EndsWith("(") && getParts[selectionIndex + 3] == ")") {
                    selectedColumn = selectedColumn.Substring(0, selectedColumn.Length - 1);
                    selectionIndex--;
                    string selectedLine = HandleParentheses(getParts, ref selectionIndex);
                    selectedColumn += selectedLine.Replace("'", "\"");
                    selectionIndex++;
                }
            }
            return selectedColumn;
        }
        private string HandleParentheses(string[] getParts, ref int selectionIndex) {
            string condition = GetConditionForParts(getParts, ref selectionIndex);
            selectionIndex = selectionIndex + 2;
            if (getParts[selectionIndex] == "as") {
                selectionIndex++;
                if (getParts[selectionIndex].Contains(",")) {
                    string AliasName = getParts[selectionIndex].Split(',')[0];
                    List<string> others = getParts[selectionIndex].Split(',').ToList();
                    others.RemoveAt(0);
                    string otherPart = string.Join(",", others);
                    condition = AliasName + " = " + "( " + condition + " )" + "," + otherPart;
                } else {
                    string AliasName = getParts[selectionIndex];
                    condition = AliasName + " = " + "( " + condition + " )";
                }
            } else {
                condition = "";
            }
            return condition;
        }
        private string HandleCaseWhenThen(string[] getParts, ref int selectionIndex) {
            string conditionCheck = "";
            string replacementCheck = "";
            string conditionTrue = "";
            string conditionFalse = "";
            string displayColumn = "";
            selectionIndex++;
            while (getParts[selectionIndex] != "then") {
                conditionCheck += getParts[selectionIndex] + " ";
                selectionIndex++;
            }
            selectionIndex++;
            while (getParts[selectionIndex] != "else") {
                conditionTrue += getParts[selectionIndex] + " ";
                selectionIndex++;
            }
            selectionIndex++;
            while (getParts[selectionIndex] != "end") {
                conditionFalse += getParts[selectionIndex] + " ";
                selectionIndex++;
            }
            foreach (var c in conditionCheck.Split(' ')) {
                string conditionModified = "";
                if (c.Contains(".")) {
                    string TableName = c.Split('.')[0];
                    string TableColumn = c.Split('.')[1];
                    string actualTableName = TableActualNameMapping[TableName];
                    string actualColumnName = GetActualTableColumnNameFromTable(actualTableName, TableColumn);
                    char tableChar = TableCharMapping[TableName];
                    conditionModified = tableChar + "." + actualColumnName;
                } else {
                    if (c == "=") {
                        conditionModified = "==";
                    } else {
                        conditionModified = c;
                    }
                }
                replacementCheck += conditionModified + " ";
            }
            string selectedLine = "";
            selectionIndex++;
            if (getParts[selectionIndex] == "as") {
                selectionIndex++;
                if (getParts[selectionIndex].Contains(",")) {
                    string CheckForComma = getParts[selectionIndex].Split(',')[0];
                    List<string> tempList = getParts[selectionIndex].Split(',').ToList();
                    tempList.RemoveAt(0);
                    string otherPart = string.Join(",", tempList);
                    displayColumn = CheckForComma;
                    selectedLine = displayColumn + " = ( " + replacementCheck + " ? " + conditionTrue + " : " + conditionFalse + " ) " + "," + otherPart;

                } else {
                    displayColumn = getParts[selectionIndex];
                    selectedLine = displayColumn + " = ( " + replacementCheck + " ? " + conditionTrue + " : " + conditionFalse + " ) ";
                }
            }
            selectionIndex++;
            return selectedLine;
        }
        private string GetJoinString(string[] getParts, List<int> innerjoinIndexList, string fromTable) {
            string JoinString = "";
            TableCharMapping[fromTable] = characterUsed++;
            if (isInnerJoinPresent) {
                List<string> innerJoinTableList = GetInnerJoinTableList(getParts, innerjoinIndexList);
                TableActualNameMapping = GetActualTableNamesForTables();
                Dictionary<string, string> JoinOnColumn = GetJoinOnColumn(getParts, innerjoinIndexList);
                foreach (var iJTable in innerJoinTableList) {
                    JoinString += Environment.NewLine + "join "
                        + TableCharMapping[iJTable] + " in " + Globals.UserDefinedObjectName + "."
                        + TableActualNameMapping[iJTable] + " on "
                        + JoinOnColumn[iJTable];
                }
            } else {
                TableActualNameMapping = GetActualTableNamesForTables();
            }
            return JoinString;
        }
        private List<string> GetInnerJoinTableList(string[] getParts, List<int> innerjoinIndexList) {
            List<string> innerJoinTableList = new List<string>();
            foreach (var innerjoinIndex in innerjoinIndexList) {
                string innerJoinTable = getParts[innerjoinIndex + 1];
                TableCharMapping[innerJoinTable] = characterUsed++;
                innerJoinTableList.Add(innerJoinTable);
            }
            return innerJoinTableList;
        }
        private Dictionary<string, string> GetJoinOnColumn(string[] getParts, List<int> innerjoinIndexList) {
            Dictionary<string, string> JoinOnColumn = new Dictionary<string, string>();
            foreach (var innerjoinIndex in innerjoinIndexList) {
                JoinOnColumn[getParts[innerjoinIndex + 1]] = GetOnColumnEquals(getParts, innerjoinIndex);
            }
            return JoinOnColumn;
        }
        private string GetOnColumnEquals(string[] getParts, int innerjoinIndex) {
            int ijBase = innerjoinIndex;
            string onColumnEquals = GetInnerJoinString(getParts, ijBase);
            int ijIndexing = ijBase + 6;
            if (getParts.ElementAtOrDefault(ijIndexing) != null) {
                while (getParts[ijIndexing] == "and" || getParts[ijIndexing] == "or") {
                    string andColumnEquals = GetAndOrColumnEqualsString(getParts, ijIndexing);
                    onColumnEquals += andColumnEquals;
                    ijIndexing = ijIndexing + 4;
                }
            }
            return onColumnEquals;
        }
        private string GetLinqSelectedColumns(List<string> selectedColumnList, string fromTable) {
            string LinqSelectedColumns = "";
            int countList = 0;
            int countOfList = selectedColumnList.Count();
            foreach (var scl in selectedColumnList) {
                countList++;
                if (scl.Contains("?")) {
                    LinqSelectedColumns += Environment.NewLine + "    " + scl;
                    string entryName = "";
                    int charLocation = scl.IndexOf("?", StringComparison.Ordinal);
                    if (charLocation > 0) {
                        entryName = scl.Substring(0, charLocation).Trim();
                    }
                    string constant = Globals.GetSubstringBetweenStrings(scl, "?", ":").Trim();
                } else if (scl.Contains("=")) {
                    LinqSelectedColumns += Environment.NewLine + "    " + scl;
                } else {
                    if (scl.Contains(".")) {
                        string tableName = scl.Split('.')[0];
                        string columnName = scl.Split('.')[1];
                        char charUsed = TableCharMapping[tableName];
                        string tablesColumn = columnName;
                        string ViewModelLine = "";
                        if (columnName.Contains("_as_")) {
                            tablesColumn = columnName.Split(new string[] { "_as_" }, StringSplitOptions.None)[0];
                            columnName = columnName.Split(new string[] { "_as_" }, StringSplitOptions.None)[1];
                            ViewModelLine = GetViewModelLineFromTable(TableActualNameMapping[tableName], tablesColumn);
                            tablesColumn = GetActualTableColumnNameFromTable(TableActualNameMapping[tableName], tablesColumn);
                        } else {
                            ViewModelLine = GetViewModelLineFromTable(TableActualNameMapping[tableName], tablesColumn);
                            tablesColumn = GetActualTableColumnNameFromTable(TableActualNameMapping[tableName], tablesColumn);
                            columnName = tablesColumn;
                        }
                        ViewModelText += Environment.NewLine + ViewModelLine;
                        string tableColumn = charUsed + "." + tablesColumn;
                        LinqSelectedColumns += Environment.NewLine + "    " + columnName + " = " + tableColumn;
                    } else {
                        if (scl.Contains("_as_")) {
                            string constant = scl.Split(new string[] { "_as_" }, StringSplitOptions.None)[0];
                            string displayName = scl.Split(new string[] { "_as_" }, StringSplitOptions.None)[1];
                            LinqSelectedColumns += Environment.NewLine + "    " + displayName + " = " + constant;
                        } else {
                            LinqSelectedColumns += Environment.NewLine + "    " + scl + " = " + TableCharMapping[fromTable] + "." + scl;
                        }
                    }
                }
                if (countList < countOfList)
                    LinqSelectedColumns += ",";
            }
            return LinqSelectedColumns;
        }
        private string GetSqlWithSpaces() {
            StringBuilder sqlText = new StringBuilder(txtSQL.Text.ToLower());
            sqlText = sqlText.Replace(Environment.NewLine, " ")
                .Replace("(", " ( ")
                .Replace(")", " ) ")
                .Replace("{", " { ")
                .Replace("}", " } ")
                .Replace("-", " - ")
                .Replace("+", " + ")
                .Replace("*", " * ")
                .Replace("/", " / ")
                .Replace(">", " > ")
                .Replace("<", " < ")
                .Replace("=", " = ")
                .Replace(" =  = ", " == ")
                .Replace(" >  = ", " >= ")
                .Replace(" <  = ", " <= ")
                .Replace("! = ", " != ")
                .Replace(" <  > ", " <> ");
            return sqlText.ToString();
        }
        private string GetInnerJoinString(string[] getParts, int innerjoinIndex) {
            int offset = 0;
            string TableAliasName = "";
            string ActualTable = "";
            if (getParts[innerjoinIndex + 3] == "on") {
                offset++;
                ActualTable = getParts[innerjoinIndex + 1];
                TableAliasName = getParts[innerjoinIndex + 1 + offset].Split('.')[0];
                TableCharMapping[TableAliasName] = characterUsed++;
                TableActualNameMapping[TableAliasName] = TableActualNameMapping[ActualTable];
            }
            string fkTable = getParts[innerjoinIndex + 3 + offset].Split('.')[0];
            string fkTableColumn = getParts[innerjoinIndex + 3 + offset].Split('.')[1];
            char fkTableChar = TableCharMapping[fkTable];
            string firstTable = (fkTable == TableAliasName) ? ActualTable : fkTable;
            string fkTableActualColumnName = GetActualTableColumnNameFromTable(TableActualNameMapping[firstTable], fkTableColumn);

            string cTable = getParts[innerjoinIndex + 5 + offset].Split('.')[0];
            string cTableColumn = getParts[innerjoinIndex + 5 + offset].Split('.')[1];
            char cTableChar = TableCharMapping[cTable];
            string secondTable = (cTable == TableAliasName) ? ActualTable : cTable;
            string cTableActualColumnName = GetActualTableColumnNameFromTable(TableActualNameMapping[secondTable], cTableColumn);

            string equality = ((getParts[innerjoinIndex + 4 + offset] == "=" || getParts[innerjoinIndex + 4 + offset] == "==") ? " equals " : ((getParts[innerjoinIndex + 4 + offset] == "<>" ? " not equals " : " like ")));
            string onColumnEquals = fkTableChar + "." + fkTableActualColumnName + equality + cTableChar + "." + cTableActualColumnName;
            return onColumnEquals;
        }
        private string GetAndOrColumnEqualsString(string[] getParts, int innerjoinIndex) {
            string afTable = getParts[innerjoinIndex + 1].Split('.')[0];
            string afTableColumn = getParts[innerjoinIndex + 1].Split('.')[1];
            char afTableChar = TableCharMapping[afTable];
            string afTableActualColumnName = GetActualTableColumnNameFromTable(TableActualNameMapping[afTable], afTableColumn);

            string asTable = getParts[innerjoinIndex + 3].Split('.')[0];
            string asTableColumn = getParts[innerjoinIndex + 3].Split('.')[1];
            char asTableChar = TableCharMapping[asTable];
            string asTableActualColumnName = GetActualTableColumnNameFromTable(TableActualNameMapping[asTable], asTableColumn);

            string andColumnEquals = ((getParts[innerjoinIndex] == "and" || getParts[innerjoinIndex] == "or") ? " where " : " where ") + afTableChar + "." + afTableActualColumnName + " == " + asTableChar + "." + asTableActualColumnName;
            return andColumnEquals;
        }
        private string GetActualTableColumnNameFromTable(string TableName, string fkTableColumn) {
            string returnActualTableNames = "";
            string filePath = Globals.EDMXworkingDirectory + @"\" + TableName + @".cs";
            if (File.Exists(filePath)) {
                string readText = File.ReadAllText(filePath);
                var fileLines = readText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                foreach (var fl in fileLines) {
                    if (fl.ToLower().Contains(" " + fkTableColumn + " ") && fl.ToLower().Contains(" { get; set; }")) {
                        string requiredColumnName = fl.Replace(" { get; set; }", "").Split(' ').Last();
                        returnActualTableNames = requiredColumnName;
                        break;
                    }
                }
            }
            return returnActualTableNames;
        }
        private string GetViewModelLineFromTable(string TableName, string fkTableColumn) {
            string v = "";
            string filePath = Globals.EDMXworkingDirectory + @"\" + TableName + @".cs";
            if (File.Exists(filePath)) {
                string readText = File.ReadAllText(filePath);
                var fileLines = readText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                foreach (var fl in fileLines) {
                    if (fl.ToLower().Contains(" " + fkTableColumn + " ") && fl.ToLower().Contains(" { get; set; }")) {
                        v = "    " + fl + @" // " + TableName;
                        break;
                    }
                }
            }
            return v;
        }
        private Dictionary<string, string> GetActualTableNamesForTables() {
            Dictionary<string, string> returnActualTableNames = new Dictionary<string, string>();
            foreach (var tablenamelower in TableCharMapping.Keys) {
                string filePath = Globals.EDMXworkingDirectory + @"\" + @"DataModel.Context.cs";
                if (File.Exists(filePath)) {
                    string readText = File.ReadAllText(filePath);
                    var fileLines = readText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                    foreach (var fl in fileLines) {
                        if (fl.ToLower().Contains("<" + tablenamelower + ">")) {
                            string tableLine = fl;
                            string actualName = tableLine.Split('<', '>')[1];
                            returnActualTableNames[tablenamelower] = actualName;
                        }
                    }
                }
            }
            return returnActualTableNames;
        }
        private void SQL2LINQ_Shown(object sender, EventArgs e) {
            while (string.IsNullOrEmpty(Globals.Server)
                || string.IsNullOrEmpty(Globals.Database)
                || string.IsNullOrEmpty(Globals.UserName)
                || string.IsNullOrEmpty(Globals.Password)) {
                ConfigureDBDetails();
                Globals.GenerateORMFiles();
                txtSearchFormName.Text = searchTableName;
            }
        }
        private void ConfigureDBDetails() {
            GetDBInformation getDBInformation = new GetDBInformation();
            getDBInformation.LoadText();
            getDBInformation.ShowDialog();
            getDBInformation.Dispose();
            if (!string.IsNullOrEmpty(Globals.UserDefinedObjectName)) {
                txtUsedObjectName.Text = Globals.UserDefinedObjectName;
            }
        }
        private void btnUpdate_Click(object sender, EventArgs e) {
            Globals.UserDefinedObjectName = txtUsedObjectName.Text;
            searchTableName = txtSearchFormName.Text;
        }
        private void btnConfigure_Click(object sender, EventArgs e) {
            ConfigureDBDetails();
        }
    }
}
