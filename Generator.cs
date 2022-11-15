namespace hell1;

public class Generator
{

    public async Task Generate(string fileName, long bytesCount)
    {
        var rnd = new Random();
        await using var binaryWriter = new StreamWriter(fileName);
        for (int i = 0; i <= bytesCount; i += sizeof(int) + sizeof(char))
        {
            int rndNumber = rnd.Next();
            binaryWriter.WriteLine(rndNumber); 
        }
    }

    
}
