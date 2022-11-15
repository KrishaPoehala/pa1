
public class Sorter
{
    public async Task Sort()
    {
        var line = "";
        var temp = "";
        var fw = new StreamWriter("A.txt");
         var br = new StreamReader("input.txt");
        temp = br.ReadLine();
        fw.WriteLine(temp);
        while ((line = br.ReadLine()) != null)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            if (int.Parse(temp) < int.Parse(line))
                fw.WriteLine(line);
            else
                fw.WriteLine("." + Environment.NewLine + line);

            temp = line;
        }
        fw.WriteLine(".");
        br.Dispose();
        await fw.DisposeAsync();

        //підрахування серій
        br = new StreamReader("A.txt");
        int count = 0;
        while ((line = br.ReadLine()) != null)
        {
            if (line.Equals("."))
                count++;
        }

        br.Dispose();
        //найближче фібоначчі (fib>count)
        var (arr, fib) = GetFibbonacciSequence(count);
        fw = new StreamWriter("A.txt",append: true);
        int k = fib - count;
        for (int i = 0; i < k; i++)
        {
            fw.WriteLine(" " + Environment.NewLine + ".");
        }
        await fw.DisposeAsync();

        int firstFibbo = Math.Min(arr[0], arr[1]), secondFibbo = Math.Abs(arr[0] - arr[1]), thirdFibbo = 0;
        await using var fw1 = new StreamWriter("B1.txt");
        await using var fw2 = new StreamWriter("B2.txt"); 
        br = new StreamReader("A.txt");
        MoveFromAToBFile(br, fw1, firstFibbo);
        MoveFromAToBFile(br, fw2, secondFibbo);
        fw1.Dispose();
        fw2.Dispose();

        //розділення усіх серій по двох файлах 
        while ((firstFibbo != 1 && secondFibbo != 0 && thirdFibbo != 0) ||
            (firstFibbo != 0 && secondFibbo != 1 && thirdFibbo != 0) ||
            (firstFibbo != 0 && secondFibbo != 0 && thirdFibbo != 1))
        {
            if (firstFibbo == 0)
            {
                await PolyphaseSort("B1.txt", "B2.txt", "B3.txt", firstFibbo, secondFibbo, thirdFibbo);
                ChangeFibConfig(ref secondFibbo, ref firstFibbo, ref thirdFibbo);
                
            }
            else if (secondFibbo == 0)
            {
                await PolyphaseSort("B2.txt", "B1.txt", "B3.txt", secondFibbo, firstFibbo, thirdFibbo);
                ChangeFibConfig(ref firstFibbo, ref secondFibbo, ref thirdFibbo);
            }
            else
            {
                await PolyphaseSort("B3.txt", "B1.txt", "B2.txt", thirdFibbo, firstFibbo, secondFibbo);
                ChangeFibConfig(ref firstFibbo, ref thirdFibbo, ref secondFibbo);
            }
        }
    }

    private static void ChangeFibConfig(ref int first, ref int second, ref int third)
    {
        if (first < third)
        {
            third -= first;
            second = first;
            first = 0;
        }
        else
        {
            first -= third;
            second = third;
            third = 0;
        }
    }

    private (int[],int) GetFibbonacciSequence(int count)
    {
        int[] arr = { 0, 1 };
        int fib = 0;
        while (count > fib)
        {
            fib = arr[0] + arr[1];
            if (arr[0] < arr[1])
                arr[0] = fib;
            else
                arr[1] = fib;
        }

        return (arr, fib);
    }

    static async Task PolyphaseSort(string firstFileName, string secondFileName, string thirdFileName,
        int fi1, int fi2, int fi3)
    {
        using var br2 = new StreamReader(secondFileName); 
        using var br3 = new StreamReader(thirdFileName);
        await using var fw1 = new StreamWriter(firstFileName);
        int min = Math.Min(fi2, fi3);
        string? line, line1, line2;
        for (int i = 0; i < min; i++)
        {
            int c = 0;
            line1 = br2.ReadLine();
            line2 = br3.ReadLine();
            while (line1?.Equals(".") == false && line2?.Equals(".") == false)
            {
                if (line1.Equals(" "))
                {
                    continue;
                }

                ++c;
                if (line2.Equals(" "))
                {
                    line2 = br3.ReadLine();
                    continue;
                }

                if (int.Parse(line1) < int.Parse(line2))
                {
                    fw1.WriteLine(line1);
                    line1 = br2.ReadLine();
                }
                else
                {
                    fw1.WriteLine(line2);
                    line2 = br3.ReadLine();
                }

            }

            if (c == 0)
            {
                fw1.WriteLine(" " + Environment.NewLine + ".");
                continue;
            }

            ReWriteSeries(line1, from: br2, to: fw1);
            ReWriteSeries(line2, from: br3, to: fw1);
            fw1.WriteLine(".");
        }

        //перезапис файлу з більшою к-стю серій (якщо 5 8 0, то 8 перезапишемо в 3)
        var fwT = new StreamWriter("BT.txt", append: false);//fale
        if (fi2 < fi3)
        {
            Move(br3, fwT, isDisposeRequired:true);
            using var brT = new StreamReader("BT.txt");
            await using var fw3 = new StreamWriter(thirdFileName, false);
            Move(from: brT, to: fw3);
        }
        else if (fi2 > fi3)
        {
            Move(from: br2, to: fwT, isDisposeRequired: true);
            var brT = new StreamReader("BT.txt");
            var fw2 = new StreamWriter(secondFileName, false);
            Move(from: brT, to: fw2, isDisposeRequired: true);
        }
        else
        {
            Move(from: br3, to: fwT, true);
            var fw3 = new StreamWriter(thirdFileName, false);
            fwT = new StreamWriter("BT.txt");
            while ((line = br2.ReadLine()) != null)
            {
                fwT.WriteLine(line);
            }
            fwT.Dispose();
            br2.Dispose();
            using var brT = new StreamReader("BT.txt");
            Move(from: brT, to: fw3);
            fw3.Dispose();
            var fw2 = new StreamWriter(secondFileName, false);
            Move(from: brT, to: fw2);
            fw2.Dispose();
        }
    }

    private static void ReWriteSeries(string? line, StreamReader from, StreamWriter to)
    {
        while (line?.Equals(".") == false)
        {
            if (!line.Equals(" "))
            {
                to.WriteLine(line);
                line = from.ReadLine();
            }
        }
    }

    private void MoveFromAToBFile(StreamReader from, StreamWriter to, int fi)
    {
        int c = 0;
        var line = "";
        while (c < fi)
        {
            line = from.ReadLine();
            to.WriteLine(line);
            if (line?.Equals(".") == true)
            {
                c++;
            }
        }
    }

    private static void Move(StreamReader from, StreamWriter to, bool isDisposeRequired = false)
    {
        var line = "";
        while (line != null)
        {
            line = from.ReadLine();
            to.WriteLine(line);
        }

        if (isDisposeRequired)
        {
            from.Dispose();
            to.Dispose();
        }
    }
}