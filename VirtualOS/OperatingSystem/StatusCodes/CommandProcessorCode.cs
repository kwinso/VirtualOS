namespace VirtualOS.OperatingSystem.StatusCodes
{
    public enum CommandProcessorCode
    {
        Processed, // Command will processed successfuly 
        RebootRequest, // User requested to reboot system
        ShutdownRequest // User requested to shutdown system    
    }
}