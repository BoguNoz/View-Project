
namespace CoreFeatures.ResposeModel
{
    /// <summary>
    /// ResponseModel class represent framework of objects return by task in classes and carries info about operation result. 
    /// It was created in order to to make easier handling any exceptions that may occur while program is running 
    /// on this layer
    /// </summary>

    public class ResponseModel<T>
    {
        /// <summary>
        /// Status represents whether the operation was successful.
        /// </summary>
        /// <returns>True if successful, false otherwise.</returns>
        public bool Status { get; set; }

        /// <summary>
        /// Message is extended message about operation result
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Result carries result of operation 
        /// </summary>
        public T Result { get; set; }

    }
}
