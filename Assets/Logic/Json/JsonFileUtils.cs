public static class FileUtils
{
    private const string BASE_FOLDER = "Assets/Resources/SongData/";

    public static string getFolderPath(string subFolder, string shortFileName)
    {
        return BASE_FOLDER + subFolder + shortFileName + "/";
    }

    public static string getFullPath(string folderPath, string shortFileName)
    {
        return folderPath + shortFileName + ".dat";
    }
}
