
namespace CoreFeatures.ResposeModel
{
    /// <summary>
    /// ResponseModel class represent framework of objects return by task in classes of View.Data Connection,  
    /// and carries info about operation result. It was created in order to to make easier handling any exceptions that may occur while program is running 
    /// on this layer
    /// </summary>

    public class ResponseModel
    {
        /// <summary>
        /// Determines whether the operation was successful.
        /// </summary>
        /// <returns>True if successful, false otherwise.</returns>
        public bool Status { get; set; }

        /// <summary>
        /// Extended message about operation result
        /// </summary>
        public string Message { get; set; } = string.Empty;


        /// <summary>
        /// Carries result of operation 
        /// </summary>
        public object Result { get; set; }

    }
}
