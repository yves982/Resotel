namespace ResotelApp.Models
{
    /// <summary>Interface for Validable Models</summary>
    /// <remarks>All models must implements this, save enums</remarks>
    interface IValidable
    {
        /// <summary>Validates an object</summary>
        /// <returns>true if the object is valid, false otherwise</returns>
        /// <remarks>This method must be called before saving or showing any implementation</remarks>
        bool Validate();
    }
}
