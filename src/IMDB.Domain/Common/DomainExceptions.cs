namespace IMDB.Domain.Common;

public class DomainException(string message) : Exception(message);

public class ForbiddenDomainException(string message) : DomainException(message);
