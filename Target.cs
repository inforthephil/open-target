using System;

public class Target
{
    public int idNo;
    public bool hit;
    public bool miss;
    private int red;
    private int green;
    private int blue;

    public Target(int id)
    {
        idNo = id;
    }

    public void setLED(int r, int g, int b)
    {
        red = r;
        green = g;
        blue = b;
    }

    string ping()
    {
        string s = idNo + ",ping";
        return s;
    }

}
