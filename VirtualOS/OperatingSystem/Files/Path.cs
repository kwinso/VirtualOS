using System;

namespace VirtualOS.OperatingSystem.Files
{
    public static class Path
    {

        public static bool IsFile(string path, string currentLocation)
        {
            path = ToAbsolutePath(path, currentLocation);
            return path.Contains(".") && !path.EndsWith("/");
        }
        public static string ToZipFormat(string path)
        {
            //  Remove slash at the begging if needed
            if (path.StartsWith("/")) path = path.Substring(1);
            // IF path does not represent a file and does not ends with slash (directories in zip have to end with slash)
            if (!path.Contains(".") && !path.EndsWith("/")) path += "/";
            return path;
        }

        public static string ToAbsolutePath(string path, string currentLocation)
        {
            
            if (path.StartsWith("..")) // One level up
            {
                string pathToGo = "";
                
                if (path.StartsWith("../"))
                    pathToGo = path.Substring(3);
                
                // Add slash at the end if it's not file to path
                pathToGo =  OneLevelUp(currentLocation) + pathToGo;

                if (!pathToGo.EndsWith("/") && !pathToGo.Contains(".")) pathToGo += "/";

                return pathToGo;
            }
            // Relative path to the current location.
            if (path.StartsWith("."))
            {
                return NavigateRelative(path, currentLocation);
            }

            // Add slash if that's a folder and have no slash at end
            if (!path.Contains(".") && !path.EndsWith("/")) path += "/";
            
            // Absolute path
            if (path.StartsWith("/")) return path;
            
            return currentLocation + path;
        }
        
        private static string OneLevelUp(string location)
        {
            return GoToParent(location);
        }
        // Navigating relative to the current directory
        private static string NavigateRelative(string path, string location)
        {
            // Stay were we were
            if (path == ".") return location;
            
            // Remove "./" from path
            var locationToGo = path.Substring(2);
            locationToGo = location + locationToGo;
            return locationToGo;
        }
        
        // /path/to/file/ => /path/to/
        private static string GoToParent(string path)
        {
            path = path.Substring(0, path.LastIndexOf("/"));
            return path.Substring(0, path.LastIndexOf("/") + 1);;
        }
    }
}