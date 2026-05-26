namespace abpd_tut_09.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string courseNotFound) :  base(courseNotFound)
    {
    }
}