using Telegram.Common.Pages;

namespace Telegram.Common.User
{
    public record UserState(Stack<IPage> Pages, UserData UserData)
    {
        public IPage CurrentPage => Pages.Peek();
        public void AddPage(IPage page)
        {
            if (CurrentPage.GetType() != page.GetType())
            {
                Pages.Push(page);
            }
        }
    }

}
