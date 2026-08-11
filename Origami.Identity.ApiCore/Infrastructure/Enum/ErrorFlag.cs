using System.Data;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Origami.Identity.Api.Core.Infrastructure
{

    using System;

    [Flags]
    public enum ErrorFlag
    {
        ERROR = 0,
        ERROR_NULLS = 2,
        OK = 1,
        ERROR_CONNECTION_FROG = 3,
        INVALID_CREDENTIALS = 100,
        INVALID_REQUEST = 101,
        REQUEST_NULL = 102,
        INVALID_TOKEN = 103,
        NOT_VALID_DATA = 104,
        UNAUTHORIZED = 105,
        NO_PERMISSIONS = 106,
        ERROR_CONNECTION_FIREBSE = 107
    }
}
