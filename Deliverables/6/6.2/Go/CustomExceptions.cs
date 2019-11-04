using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CustomExceptions
{
    public class InvalidJsonInputException : Exception
    {
        public InvalidJsonInputException()
        {
        }

        public InvalidJsonInputException(string message)
            : base(message)
        {
        }

        public InvalidJsonInputException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class WrapperException : Exception
    {
        public WrapperException()
        {
        }

        public WrapperException(string message)
            : base(message)
        {
        }

        public WrapperException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class RuleCheckerException : Exception
    {
        public RuleCheckerException()
        {
        }

        public RuleCheckerException(string message)
            : base(message)
        {
        }

        public RuleCheckerException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class BoardException : Exception
    {
        public BoardException()
        {
        }

        public BoardException(string message)
            : base(message)
        {
        }

        public BoardException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class PlayerException : Exception
    {
        public PlayerException()
        {
        }

        public PlayerException(string message)
            : base(message)
        {
        }

        public PlayerException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class RefereeException : Exception
    {
        public RefereeException()
        {
        }

        public RefereeException(string message)
            : base(message)
        {
        }

        public RefereeException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
