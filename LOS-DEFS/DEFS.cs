using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using LOS_Security;

namespace LOS_DEFS
{
    /// <summary>
    /// The Distributed Encrypted Filesystem
    /// </summary>
    public class DEFS
    {

        public enum UserStatus
        {
            UserExists,
            UserDoesNotExist,
            UserPanic
        }


        // User File Data Structure
        private byte[] _memorableWordStructure = new byte[8];
        // The location of the Memorable Word as an Offset to the end of the User File Data Structure.

        private byte[] _panicWordStructure = new byte[8];
        // The location of the Panic Word as an Offset to the end of the User File Data Structure.

        private byte[] _distributedFileSystemId = new byte[10];
        // The location of the Distributed File System ID as an Offset to the end of the User File Data Structure.
        private int UserFileDataStructureSize = 27;


        // Special Folders' Paths
        private const string RootFolder = "C:\\LOS";
        private const string UserFilesFolder = "C:\\LOS\\UF";
        // ReSharper disable once InconsistentNaming
        private const string DEFSFolder = "C:\\LOS\\DEFS";

        public DEFS()
        {
            // Check to see if the directory structure has been created
            // If not create it now
            if (!Directory.Exists(RootFolder)) Directory.CreateDirectory(RootFolder); // LOS ROOT Folder
            if (!Directory.Exists(UserFilesFolder)) Directory.CreateDirectory(UserFilesFolder); // LOS User Files Folder
            if (!Directory.Exists(DEFSFolder)) Directory.CreateDirectory(DEFSFolder); // LOS DEFS Folder
        }


        /// <summary>
        /// Generate the Filesystem
        /// </summary>
        /// <param name="mb">
        /// int: The Number of MBytes that the Filesystem can use
        /// </param>
        /// <returns>
        /// bool: True the filesystem was successfully created
        /// bool: False the filesystem failed to be create
        /// </returns>
        /// <remarks>
        /// Generate 100 Encrypted User Files
        /// </remarks>
        // ReSharper disable once InconsistentNaming
        public bool CreateDEFS(int mb)
        {
            Int64 fileByteSize = (mb / 100) * 1024;
            List<string> filenamesList = new List<string>();
            filenamesList = GenerateFilenames();
            Parallel.ForEach(filenamesList, (currentFile) =>
                {
                    GenerateFile(currentFile, fileByteSize);
                }
            );

            return true;
        }

        private void GenerateFile(string currentFile, long fileByteSize)
        {
            // Create the full filename path
            string fullPath = Path.Combine(UserFilesFolder, currentFile);
            // Create the random encrypted file contents
            string fileContents = GenerateFileContents(fileByteSize);
            // Finally save the file to the drive
            File.WriteAllText(fullPath, fileContents); // This may cause a locking problem TODO: Check this
        }

        /// <summary>
        /// Create the Encrypted contents of the file
        /// </summary>
        /// <param name="fileByteSize">
        /// long: The number of bytes that the file should contain
        /// </param>
        /// <returns>
        /// string: The encrypted string of fileByteSize
        /// </returns>
        private string GenerateFileContents(long fileByteSize)
        {
            char[] chars = new char[62];
            chars =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[fileByteSize];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder();
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }

            // We are never going to decrypt this file so do not care about the encryption password.
            return StringCipher.Encrypt(result.ToString(), GenerateRandomPassword());
        }

        /// <summary>
        /// Generate a Random password for encrypting the empty UserFile
        /// </summary>
        /// <returns>
        /// string: The Random Password
        /// </returns>
        private string GenerateRandomPassword()
        {
            char[] chars = new char[62];
            chars =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[20];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder();
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();

        }


        /// <summary>
        /// Create the filenames required by CreateDEFS
        /// </summary>
        /// <returns>
        /// List of type string: the 100 filenames to be used as User Files
        /// </returns>
        private List<string> GenerateFilenames()
        {
            Random random = new Random();
            int fileNameCount = random.Next(5, 20);
            List<string> filenamesList = new List<string>();
            for (int idx = 0; idx < fileNameCount; idx++)
            {
                string g = Guid.NewGuid().ToString();
                filenamesList.Add(g.Replace("-", ""));
            }
            return filenamesList;
        }

        /// <summary>
        /// Does the user Exist?
        /// </summary>
        /// <param name="username">
        /// SecureString: The user's username
        /// </param>
        /// <param name="password">
        /// SecureString: The user's password 
        /// </param>
        /// <param name="memorableWord">
        /// SecureString: The user's memorable word
        /// </param>
        /// <returns>
        /// UserStatus: UserFound -> The User has been found on the system
        /// UserStatus: UserNotFound -> The User has NOT been found on the system
        /// UserStatus: UserPanic -> Destroy the User's Data
        /// </returns>
        public UserStatus FindUser(SecureString username, SecureString password, SecureString memorableWord)
        {
            foreach (string filePath in GetUserFilePaths())
            {
                UserStatus us = SearchForUserInUserFile(filePath, username, password, memorableWord);
                if (us == UserStatus.UserExists) return UserStatus.UserExists;
                if (us == UserStatus.UserPanic) return UserStatus.UserPanic;
            }

            return UserStatus.UserDoesNotExist;
        }

        /// <summary>
        /// Searches the filePath file to see if the user exists in it.
        /// </summary>
        /// <param name="filePath">
        /// string: The UserFile to search
        /// </param>
        /// <param name="username">
        /// SecureString: The User's username        
        /// </param>
        /// <param name="password">
        /// SecureString: The User's password
        /// </param>
        /// <param name="memorableWord">
        /// SecureString: The User's MemorableWord
        /// </param>
        /// <returns>
        /// Enum: UserStatus UserExists -> The User has been found
        /// Enum: UserStatus UserDoesNotExist -> The User has NOT been found
        /// Enum: UserStatus UserPanic -> Destroy All Data
        /// </returns>
        private UserStatus SearchForUserInUserFile(string filePath, SecureString username, SecureString password, SecureString memorableWord)
        {
            int fileOffset = GetFileOffset(username, password);
            FileStream fs = null;
            StreamReader sr = null;

            try
            {
                fs = new FileStream(Path.Combine(UserFilesFolder, filePath), FileMode.Open, FileAccess.Read,
                    FileShare.Read);
                sr = new StreamReader(fs);
                char[] buffer = new char[8];
                sr.Read(buffer, fileOffset, 8);
                string unsafePassphrase = username.ConvertToUnsecureString();
                unsafePassphrase += password.ConvertToUnsecureString();

                StringBuilder sb = new StringBuilder();
                foreach (char c in buffer)
                {
                    sb.Append(c.ToString());
                }

                string result = StringCipher.Decrypt(sb.ToString(), unsafePassphrase);
                unsafePassphrase.Erase();
                int lookupPosition = -1;
                bool successfulConversion = int.TryParse(result, out lookupPosition);
                if (successfulConversion)
                {
                    if (SearchForMemorableWord(fs, memorableWord, lookupPosition, username, password))
                    {
                        return UserStatus.UserExists;
                    }
                    else if (SearchForMemorableWord(fs, memorableWord, lookupPosition + 8, username, password))
                    {
                        return UserStatus.UserPanic;
                    }
                    else
                    {
                        return UserStatus.UserDoesNotExist;
                    }
                }

                fs.Close();
                sr.Close();
            }
            catch (Exception)
            {
                //TODO LOG THIS
            }
            finally
            {
                username.Dispose();
                password.Dispose();
                memorableWord.Dispose();
                sr?.Close();
                fs?.Close();
            }

            return UserStatus.UserDoesNotExist;
        }


        /// <summary>
        /// Searches the UserFile for the memorableWord
        /// </summary>
        /// <param name="fs">
        /// FileStream: The file to be searched
        /// </param>
        /// <param name="memorableWord">
        /// SecureString: The Memorable Word to be found
        /// </param>
        /// <param name="lookupPosition">
        /// int: The offset in the file to start searching from
        /// </param>
        /// <param name="username">
        /// SecureString: The User's Username
        /// </param>
        /// <param name="password">
        /// SecureString: The User's Password
        /// </param>
        /// <returns>
        /// bool: True if the memorable word is found
        /// bool: False if the memorable word is NOT found
        /// </returns>
        private bool SearchForMemorableWord(FileStream fs, SecureString memorableWord, int lookupPosition, SecureString username, SecureString password)
        {


            StreamReader sr = null;

            try
            {

                sr = new StreamReader(fs);
                char[] buffer = new char[8];
                sr.Read(buffer, lookupPosition, 8);
                string unsafePassphrase = username.ConvertToUnsecureString();
                unsafePassphrase += password.ConvertToUnsecureString();

                StringBuilder sb = new StringBuilder();
                foreach (char c in buffer)
                {
                    sb.Append(c.ToString());
                }

                string result = StringCipher.Decrypt(sb.ToString(), unsafePassphrase);
                unsafePassphrase.Erase();
                string unsafeMemorableWord = memorableWord.ConvertToUnsecureString();
                if (result == unsafeMemorableWord)
                {
                    unsafeMemorableWord.Erase();
                    fs.Close();
                    sr.Close();
                    return true;
                }

                unsafeMemorableWord.Erase();
                fs.Close();
                sr.Close();
                return false;
            }
            catch (Exception)
            {

                //TODO LOG THIS
            }
            finally
            {
                username.Dispose();
                password.Dispose();
                memorableWord.Dispose();
                sr?.Close();
                fs?.Close();
            }

            return false;
        }


        /// <summary>
        /// Get the offset location in the UserFile where we will find the User Structure
        /// </summary>
        /// <param name="username">
        /// SecureString: The User's username  
        /// </param>
        /// <param name="password">
        /// SecureString: The User's password
        /// </param>
        /// <param name="memorableWord">
        /// SecureString: The User's MemorableWord
        /// </param>
        /// <returns>
        /// int: The offset location
        /// </returns>
        private int GetFileOffset(SecureString username, SecureString password)
        {
            // Convert the secure username and password to unsafe strings
            string unsafeString = GetUnsafeString(username);
            unsafeString += GetUnsafeString(password);

            // Calculate their numeric values (This is the offset)
            int totalValue = 0;
            foreach (char c in unsafeString)
            {
                totalValue += (int)c;
            }
            unsafeString = string.Empty;

            return totalValue;
        }

        /// <summary>
        /// Converts a SafeString to an UnsafeString
        /// </summary>
        /// <param name="safeSecureString">
        /// SecureString: The SecureString to convert
        /// </param>
        /// <returns>
        /// string: The unsafe decrypted string required
        /// </returns>
        private string GetUnsafeString(SecureString safeSecureString)
        {
            return safeSecureString.ConvertToUnsecureString();

        }

        /// <summary>
        /// Get all the UserFile filenames
        /// </summary>
        /// <returns>
        /// IEnumerable of type string: Collection of enumerable strings of the UserFile Filenames
        /// </returns>
        private IEnumerable<string> GetUserFilePaths()
        {
            return Directory.EnumerateFiles(UserFilesFolder, "*", SearchOption.TopDirectoryOnly);

        }
    }


}
