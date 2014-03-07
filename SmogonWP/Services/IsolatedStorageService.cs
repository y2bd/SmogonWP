using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;

namespace SmogonWP.Services
{
  public class IsolatedStorageService
  {
    private const string RootDir = "data";

    // used to avoid read race conditions
    private readonly object _readLock = new object();

    public async Task WriteStringToFileAsync(string filename, string content, bool overwriteIfExists = true)
    {
      ensureRootDirExists();

      await Task.Run(() =>
      {
        var filePath = buildPath(filename);

        using (var isf = IsolatedStorageFile.GetUserStoreForApplication())
        {
          if (isf.FileExists(filePath))
          {
            if (overwriteIfExists) isf.DeleteFile(filePath);
            else return;
          }

          using (var stream = isf.CreateFile(filePath))
          using (var writer = new StreamWriter(stream))
          {
            writer.Write(content);
          }
        }
      });
    }

    public async Task<string> ReadStringFromFileAsync(string filename)
    {
      ensureRootDirExists();

      return await Task.Run(() =>
      {
        string contents;

        var filePath = buildPath(filename);

        using (var isf = IsolatedStorageFile.GetUserStoreForApplication())
        {
          try
          {
            using (var filestream = isf.OpenFile(filePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(filestream))
            {
              contents = reader.ReadToEnd();
            }
          }
          catch (IsolatedStorageException)
          {
            contents = null;
          }
          catch (IOException)
          {
            contents = null;
          }
        }

        return contents;
      });
    }

    public async Task<bool> FileExistsAsync(string filename)
    {
      ensureRootDirExists();

      return await Task.Run(() =>
      {
        bool exists;

        var filePath = buildPath(filename);

        using (var isf = IsolatedStorageFile.GetUserStoreForApplication())
        {
          exists = isf.FileExists(filePath);
        }

        return exists;
      });
    }

    private void ensureRootDirExists()
    {
      using (var isf = IsolatedStorageFile.GetUserStoreForApplication())
      {
        if (!isf.DirectoryExists(RootDir)) isf.CreateDirectory(RootDir);
      }
    }

    private string buildPath(string filename)
    {
      return Path.Combine(RootDir, filename);
    }
  }
}
