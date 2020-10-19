
namespace RightPoint.Business
{
    public interface IMovies
    {
        Models.Movies GetMovies(string searchTitle, string searchType, int pageNumber);
    }
}