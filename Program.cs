using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace SharableSpreadSheet
{
    class SharableSpreadsheet
    {
        private int numOfRows;
        private int numOfCols;
        private string[,] spreadsheet;

        //private Mutex rw_mutex = new Mutex();
        //private Mutex mutex = new Mutex();

        SemaphoreSlim rw_mutex = new SemaphoreSlim(1);
        SemaphoreSlim mutex = new SemaphoreSlim(1);

        int read_count = 0;
        int max_read_count = -1;

        public SharableSpreadsheet(int nRows, int nCols)
        {
            // construct a nRows*nCols spreadsheet
            string[,] spreadsheet = new string[nRows, nCols];
            numOfRows = nRows;
            numOfCols = nCols; 
            for (int i = 0; i < numOfRows; i++)
            {
                for(int j = 0; j < numOfCols; j++)
                {
                    spreadsheet[i, j] = "";
                }
            }
            this.spreadsheet = spreadsheet;
        }

        //reader    
        public void print()
        {
            mutex.Wait();
            if (max_read_count > -1)
            {
                while (read_count >= max_read_count)
                {
                    //waiting
                }
            }
            read_count++;
            //first reader enterted 
            if (read_count == 1)
                rw_mutex.Wait();
            mutex.Release();


            for (int i = 0; i < numOfRows; i++)
            {
                for( int j = 0; j < numOfCols; j++)
                {
                    Console.Write(spreadsheet[i, j]+" ");
                }
                Console.WriteLine();
            }

            mutex.Wait();
            read_count--;
            //last reader 
            if (read_count == 0)
                rw_mutex.Release();
            mutex.Release();

        }

        //reader
        public String getCell(int row, int col)
        {
            mutex.Wait();
            if (max_read_count > -1)
            {
                while (read_count >= max_read_count)
                {
                    //waiting
                }
            }
            read_count++;
            //first reader enterted 
            if (read_count == 1)
                rw_mutex.Wait();
            mutex.Release();


            // return the string at [row,col]
            string toReturn = spreadsheet[row, col];


            mutex.Wait();
            read_count--;
            //last reader 
            if (read_count == 0)
                rw_mutex.Release();
            mutex.Release();

            return toReturn;
        }

        //writer
        public void setCell(int row, int col, String str)
        {
            // set the string at [row,col]
            rw_mutex.Wait();

            spreadsheet[row, col] = str;

            rw_mutex.Release();

        }
        
        //reader
        public Tuple<int, int> searchString(String str)
        {
            mutex.Wait();
            if (max_read_count > -1)
            {
                while (read_count >= max_read_count)
                {
                    //waiting
                }
            }
            read_count++;
            //first reader enterted 
            if (read_count == 1)
                rw_mutex.Wait();
            mutex.Release();


            // search the cell with string str, and return true/false accordingly.
            // return first cell indexes that contains the string (search from first row to the last row)
            Tuple<int, int> toReturn = new Tuple<int, int>(-1,-1);
            for (int i = 0; i < numOfRows; i++)
            {
                for(int j = 0; j < numOfCols; j++)
                {
                    if(spreadsheet[i, j] == str)
                    {
                        toReturn = new Tuple<int, int>(i, j);
                    }
                }
            }


            mutex.Wait();
            read_count--;
            //last reader 
            if (read_count == 0)
                rw_mutex.Release();
            mutex.Release();
            return toReturn;
        }

        //writer
        public void exchangeRows(int row1, int row2)
        {
            // exchange the content of row1 and row2
            rw_mutex.Wait();
            string [] temp = new string[numOfCols];
            for(int j = 0; j < numOfCols; j++)
            {
                temp[j] = spreadsheet[row1, j];
            }

            for (int j = 0; j < numOfCols; j++)
            {
                spreadsheet[row1, j] = spreadsheet[row2, j];
            }

            for (int j = 0; j < numOfCols; j++)
            {
                spreadsheet[row2, j] = temp[j];
            }
            rw_mutex.Release();
        }

        //writer
        public void exchangeCols(int col1, int col2)
        {
            rw_mutex.Wait();
            // exchange the content of col1 and col2
            string[] temp = new string[numOfRows];
            for (int i = 0; i < numOfRows; i++)
            {
                temp[i] = spreadsheet[i, col1];
            }

            for (int i = 0; i < numOfRows; i++)
            {
                spreadsheet[i, col1] = spreadsheet[i, col2];
            }

            for (int i = 0; i < numOfRows; i++)
            {
                spreadsheet[i, col2] = temp[i];
            }
            rw_mutex.Release();
        }

        //reader
        public int searchInRow(int row, String str)
        {
            mutex.Wait();
            if (max_read_count > -1)
            {
                while (read_count >= max_read_count)
                {
                    //waiting
                }
            }
            read_count++;
            //first reader enterted 
            if (read_count == 1)
                rw_mutex.Wait();
            mutex.Release();


            // perform search in specific row
            int col = -1;
            for (int j = 0; j < numOfCols; j++)
            {
                if (spreadsheet[row, j] == str)
                {
                    col = j;
                }
            }
            int toReturn = col;


            mutex.Wait();
            read_count--;
            //last reader 
            if (read_count == 0)
                rw_mutex.Release();
            mutex.Release();

            return toReturn;
        }

        //reader
        public int searchInCol(int col, String str)
        {
            mutex.Wait();
            if (max_read_count > -1)
            {
                while (read_count >= max_read_count)
                {
                    //waiting
                }
            }
            read_count++;
            //first reader enterted 
            if (read_count == 1)
                rw_mutex.Wait();
            mutex.Release();


            // perform search in specific col
            int row = -1;
            for (int i = 0; i < numOfRows; i++)
            {
                if (spreadsheet[i, col] == str)
                {
                    row = i;
                }
            }
            int toReturn = row;


            mutex.Wait();
            read_count--;
            //last reader 
            if (read_count == 0)
                rw_mutex.Release();
            mutex.Release();
            return toReturn;
        }


        //reader
        public Tuple<int, int> searchInRange(int col1, int col2, int row1, int row2, String str)
        {
            mutex.Wait();
            if (max_read_count > -1)
            {
                while (read_count >= max_read_count)
                {
                    //waiting
                }
            }
            read_count++;
            //first reader enterted 
            if (read_count == 1)
                rw_mutex.Wait();
            mutex.Release();


            // perform search within spesific range: [row1:row2,col1:col2] 
            //includes col1,col2,row1,row2
            Tuple<int, int> toReturn = new Tuple<int, int>(-1,-1);
            for (int i = row1; i <= row2; i++)
            {
                for (int j = col1; j <= col2; j++)
                {
                    if (spreadsheet[i, j] == str)
                    {
                        toReturn = new Tuple<int, int>(i, j);
                    }
                }
            }

            mutex.Wait();
            read_count--;
            //last reader 
            if (read_count == 0)
                rw_mutex.Release();
            mutex.Release();
            return toReturn;
        }

        //writer
        public void addRow(int row1)
        {
            //add a row after row1
            rw_mutex.Wait();
            string[,] newSpreadsheet = new string[numOfRows+1, numOfCols];

            for(int i = 0; i <= row1; i++)
            {
                for(int j = 0; j < numOfCols; j++)
                {
                    newSpreadsheet[i, j] = spreadsheet[i, j];
                }
            }
            for (int j = 0; j < numOfCols; j++)
            {
                newSpreadsheet[row1 + 1, j] = "";
            }
            for (int i = row1+1; i < numOfRows; i++)
            {
                for (int j = 0; j < numOfCols; j++)
                {
                    newSpreadsheet[i+1, j] = spreadsheet[i, j];
                }
            }
            numOfRows++;
            spreadsheet = newSpreadsheet;
            rw_mutex.Release();
        }

        //writer
        public void addCol(int col1)
        {
            //add a column after col1
            rw_mutex.Wait();
            string[,] newSpreadsheet = new string[numOfRows, numOfCols+1];

            for (int i = 0; i < numOfRows; i++)
            {
                for (int j = 0; j <= col1; j++)
                {
                    newSpreadsheet[i, j] = spreadsheet[i, j];
                }
            }

            for (int i = 0; i < numOfRows; i++)
            {
                newSpreadsheet[i, col1+1] = "";
            }

            for (int i = 0; i < numOfRows; i++)
            {
                for (int j = col1+1; j < numOfCols; j++)
                {
                    newSpreadsheet[i, j+1] = spreadsheet[i, j];
                }
            }
            numOfCols++;
            spreadsheet = newSpreadsheet;
            rw_mutex.Release();
        }

        //reader
        public Tuple<int, int>[] findAll(String str, bool caseSensitive)
        {
            mutex.Wait();
            if (max_read_count > -1)
            {
                while (read_count >= max_read_count)
                {
                    //waiting
                }
            }
            read_count++;
            //first reader enterted 
            if (read_count == 1)
                rw_mutex.Wait();
            mutex.Release();


            List<Tuple<int, int>> listOfTuples = new List<Tuple<int,int>>();
            // perform search and return all relevant cells according to caseSensitive param
            for (int i = 0; i < numOfRows; i++)
            {
                for (int j = 0; j < numOfCols; j++)
                {
                    if (caseSensitive)
                    {
                        if (spreadsheet[i, j] == str)
                        {
                            listOfTuples.Add(new Tuple<int, int>(i, j));
                        }
                    }
                    else
                    {
                        if (spreadsheet[i, j].ToLower() == str.ToLower())
                        {
                            listOfTuples.Add(new Tuple<int, int>(i, j));
                        }
                    }
                }
            }
            
            mutex.Wait();
            read_count--;
            //last reader 
            if (read_count == 0)
                rw_mutex.Release();
            mutex.Release();
            return listOfTuples.ToArray();
        }

        //writer
        public void setAll(String oldStr, String newStr, bool caseSensitive)
        {
            // replace all oldStr cells with the newStr str according to caseSensitive param
            rw_mutex.Wait();
            for (int i = 0; i < numOfRows; i++)
            {
                for (int j = 0; j < numOfCols; j++)
                {
                    if (caseSensitive)
                    {
                        if (spreadsheet[i, j] == oldStr)
                        {
                            spreadsheet[i,j]=newStr;
                        }
                    }
                    else
                    {
                        if (spreadsheet[i, j].ToLower() == oldStr.ToLower())
                        {
                            spreadsheet[i, j] = newStr;
                        }
                    }
                }
            }
            rw_mutex.Release();
        }

        //reader
        public Tuple<int, int> getSize()
        {
            mutex.Wait();
            if (max_read_count > -1)
            {
                while (read_count >= max_read_count)
                {
                    //waiting
                }
            }
            read_count++;
            //first reader enterted 
            if (read_count == 1)
                rw_mutex.Wait();
            mutex.Release();


            // return the size of the spreadsheet in nRows, nCols
            Tuple<int,int> toReturn = new Tuple<int,int>(numOfRows, numOfCols);


            mutex.Wait();
            read_count--;
            //last reader 
            if (read_count == 0)
                rw_mutex.Release();
            mutex.Release();
            return toReturn;
        }

        //writer + reader
        public void setConcurrentSearchLimit(int nUsers)
        {
            // this function aims to limit the number of users that can perform the search operations concurrently.
            // The default is no limit. When the function is called, the max number of concurrent search operations is set to nUsers. 
            // In this case additional search operations will wait for existing search to finish.
            // This function is used just in the creation
            rw_mutex.Wait();
            mutex.Wait();
            max_read_count = nUsers;
            mutex.Release();
            rw_mutex.Release();
        }

        //reader
        public void save(String fileName)
        {
            mutex.Wait();
            if (max_read_count > -1)
            {
                while (read_count >= max_read_count)
                {
                    //waiting
                }
            }
            read_count++;
            //first reader enterted 
            if (read_count == 1)
                rw_mutex.Wait();
            mutex.Release();


            // save the spreadsheet to a file fileName.
            // you can decide the format you save the data. There are several options.
            using (StreamWriter outfile = new StreamWriter(fileName))
            {
                for (int i = 0; i < numOfRows; i++)
                {
                    for (int j = 0; j < numOfCols; j++)
                    {
                        if (j == numOfCols - 1)
                        {
                            outfile.Write(spreadsheet[i, j]);
                        }
                        else
                        {
                            outfile.Write(spreadsheet[i, j] + ',');
                        }
                    }
                    //trying to write data to csv
                    outfile.WriteLine();
                }
            }


            mutex.Wait();
            read_count--;
            //last reader 
            if (read_count == 0)
                rw_mutex.Release();
            mutex.Release();
        }

        //writer
        public void load(String fileName)
        {
            rw_mutex.Wait();


            // load the spreadsheet from fileName
            // replace the data and size of the current spreadsheet with the loaded data
            string[][] newSpreadsheet = File.ReadLines(fileName).Select(x => x.Split(',')).ToArray();

            numOfRows=newSpreadsheet.Length;
            if (numOfRows > 0)
            {
                numOfCols = newSpreadsheet[0].Length;
            }
            else
            {
                numOfCols = 0;
            }

            string[,] newSpreadsheetToChange = new string[numOfRows, numOfCols];

            for (int i = 0; i < numOfRows; i++)
            {
                for(int j = 0; j < numOfCols; j++)
                {
                    newSpreadsheetToChange[i, j] = newSpreadsheet[i][j];
                }
            }

            spreadsheet = newSpreadsheetToChange;

            rw_mutex.Release();
        }
        
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            /*SharableSpreadsheet ss = new SharableSpreadsheet(3, 3);

            Console.WriteLine("SET CELL");
            ss.setCell(0, 0, "a");
            ss.setCell(1, 1, "b");

            Console.WriteLine("GET CELL");
            Console.WriteLine(ss.getCell(0, 0));
            Console.WriteLine(ss.getCell(1, 1));
            Console.WriteLine(ss.getCell(1, 2));


            Console.WriteLine("SEARCH STRING");
            Console.WriteLine(ss.searchString("a"));
            Console.WriteLine(ss.searchString("b"));
            Console.WriteLine(ss.searchString("c"));

            Console.WriteLine("Exchange rows");
            ss.print();
            ss.exchangeRows(0, 1);
            ss.print();


            Console.WriteLine("Exchange cols");
            ss.print();
            ss.exchangeCols(0, 1);
            ss.print();

            Console.WriteLine("SEARCH in row");
            Console.WriteLine(ss.searchInRow(0, "a"));
            Console.WriteLine(ss.searchInRow(0, "b"));

            Console.WriteLine("SEARCH in col");
            
            Console.WriteLine(ss.searchInCol(0, "a"));
            Console.WriteLine(ss.searchInCol(0, "b"));

            Console.WriteLine("SEARCH in range");
            ss.setCell(0, 1, "b");
            Console.WriteLine(ss.searchInRange(0, 1, 0, 1, "b"));

            Console.WriteLine("add row");
            ss.setCell(2, 2, "c");
            ss.print();
            ss.addRow(1);
            ss.setCell(2, 0, "d");
            ss.print();

            Console.WriteLine("add col");
            ss.print();
            ss.addCol(1);
            ss.setCell(0, 2, "e");
            Console.WriteLine("new");
            ss.print();
            
            Console.WriteLine("find all");
            ss.setCell(3,3, "A");
            ss.print();
            foreach (Tuple<int,int> tuple in ss.findAll("a", true)){
                Console.WriteLine(tuple);
            }


            Console.WriteLine("set all");
            ss.print();
            Console.WriteLine("new");
            ss.setAll("a", "B", false);
            ss.print();


            Console.WriteLine(ss.getSize());

            //ss.save("spreadsheet.csv");

            ss.load("spreadsheet.csv");

            ss.print();
            Console.WriteLine(ss.getSize());*/

            SharableSpreadsheet sheet = new SharableSpreadsheet(5,5);
            Thread th1 = new Thread(() => sheet.setCell(0, 0, "a"));
            Thread th2 = new Thread(() => sheet.setCell(1, 1, "b"));
            Thread th3 = new Thread(() => sheet.setCell(2, 2, "c"));

            Thread th4 = new Thread(() => Console.WriteLine(sheet.getCell(0, 0)));
            Thread th5 = new Thread(() => Console.WriteLine(sheet.getCell(1, 1)));
            Thread th6 = new Thread(() => Console.WriteLine(sheet.getCell(2, 2)));
            Thread th7 = new Thread(() => sheet.load("spreadsheet.csv"));

            th1.Start();
            th2.Start();
            th3.Start();
            th4.Start();
            th5.Start();
            th6.Start();
            th7.Start();

            th1.Join();
            th2.Join();
            th3.Join();
            th4.Join();
            th5.Join();
            th6.Join();
            th7.Join();

            sheet.print();
        }
    }
}
