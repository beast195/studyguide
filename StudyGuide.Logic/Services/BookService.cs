using Microsoft.Office.Interop.Word;
using StudyGuide.Framework.Core.Boundaries;
using StudyGuide.Framework.Core.Models;
using StudyGuide.Framework.Repositories;
using StudyGuide.Logic.Boundaries;
using StudyGuide.Logic.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace StudyGuide.Logic.Services
{
    public class BookService : IBookService
    {
        private readonly IBaseRepository<Book> _bookReposistory;

        public BookService(BaseRepository<Book> bookReposistory)
        {
            _bookReposistory = bookReposistory;
        }

        public async System.Threading.Tasks.Task CreateBook(string filename)
        {
            object missing = null;
            var app = new ApplicationClass();
            var doc = app.Documents.OpenNoRepairDialog(filename, missing, missing, missing) as DocumentClass;
            var numOfWords = doc.Words.Count;
            var chapterCount = numOfWords / 10; // chapters
            var chapterNumber = 1;
            var chapter = doc.Range(chapterCount - chapterCount * chapterNumber, chapterCount * chapterNumber);
            var chapterContent = new List<string>();
            foreach (Range sentence in chapter.Sentences)
            {
                foreach (Range word in sentence.Words)
                {
                    if (word.Bold == -1)
                    {
                        chapterContent.Add(sentence.Text);
                        break;
                    }
                }
            }
            doc.Close();
            app.Quit();

            var bookContent = "";
            chapterContent.ForEach(c => bookContent += c);

            var book = new Book();
            book.BookFileName = filename;
            book.BookOpened = true;
            book.BookTitle = filename.Split('\\').Last().Split('.').First();
            book.BookType = BookType.WORD;
            book.ChaptersRead = 0;
            book.ChapterTotal = chapterCount;
            book.LastRead = DateTime.Now;

            var chapterDB = new Chapter();
            chapterDB.ChapterOpened = true;
            chapterDB.Content = bookContent;
            chapterDB.LastRead = DateTime.Now;
            chapterDB.Number = 1;
            book.Chapters = new HashSet<Chapter>() { chapterDB };

            _bookReposistory.Add(book);
            await _bookReposistory.SaveAsync();
        }

        public ChapterEntity GetOpenBook()
        {
            var book = _bookReposistory.Where(c => c.BookOpened && c.Chapters.Where(ch => ch.ChapterOpened).Any()).AsQueryable().Include(b => b.Chapters).FirstOrDefault();
            var content = book.Chapters.Where(c => c.ChapterOpened).FirstOrDefault();
            var chapterEntity = new ChapterEntity
            {
                Content = content.Content,
                BookName = book.BookTitle
            };
            return chapterEntity;
        }

        public List<BookEntity> GetAllOpenBooks()
        {
            var books = _bookReposistory.Where(c => c.BookOpened && c.Chapters.Where(ch => ch.ChapterOpened).Any()).AsQueryable().Include(b => b.Chapters).Select(b =>
            new BookEntity
            {
                BookId = b.Id,
                BookName = b.BookTitle,
                NumberOfChapters = b.ChapterTotal,
                Open = b.BookOpened,
                Chapers = b.Chapters.Select(ch =>
                    new ChapterEntity
                    {
                        BookName = b.BookTitle,
                        Content = ch.Content,
                        Id = ch.Id,
                        ChapterNumber = ch.Number
                    }).ToList()
            }).ToList();

            return books;
        }

        public List<BookEntity> GetAllBooks()
        {
            var books = _bookReposistory.Where(c => c.Chapters.Any()).AsQueryable().Include(b => b.Chapters).Select(b =>
            new BookEntity
            {
                BookId = b.Id,
                BookName = b.BookTitle,
                NumberOfChapters = b.ChapterTotal,
                Open = b.BookOpened,
                Chapers = b.Chapters.Select(ch =>
                    new ChapterEntity
                    {
                        BookName = b.BookTitle,
                        Content = ch.Content,
                        Id = ch.Id,
                        ChapterNumber = ch.Number,
                        OpenChap = ch.ChapterOpened
                    }).ToList()
            }).ToList();

            return books;
        }


        public async System.Threading.Tasks.Task OpenAChapter(int chapId)
        {
            var books = _bookReposistory.Where(c => c.BookOpened && c.Chapters.Where(ch => ch.ChapterOpened).Any())
                            .AsQueryable().Include(b => b.Chapters);
            var bookUpdate = _bookReposistory.Where(c => c.Chapters.Where(ch => ch.Id == chapId).Any()).AsQueryable().Include(b => b.Chapters);
            foreach (var book in books)
            {
                book.BookOpened = false;

                book.Chapters = CloseChappter(book.Chapters);
            }

            foreach (var book in bookUpdate)
            {
                book.BookOpened = true;
                book.Chapters = CloseChappter(book.Chapters, chapId);
            }

            bookUpdate.ToList().AddRange(books.AsEnumerable());
            _bookReposistory.UpdateAll(new HashSet<Book>(bookUpdate));
            await _bookReposistory.SaveAsync();
        }

        private HashSet<Chapter> CloseChappter(ISet<Chapter> chapters, int? chapId = null)
        {
            var chaps = new HashSet<Chapter>();
            foreach (var chapter in chapters)
            {
                chapter.ChapterOpened = false;
                if (chapId != null)
                {
                    if (chapId == chapter.Id)
                    {
                        chapter.ChapterOpened = true;
                    }
                }
                chaps.Add(chapter);
            }

            return chaps;
        }

        public async System.Threading.Tasks.Task OpenNewChapter(Book book, int chapterNumber)
        {
            object missing = null;
            var app = new ApplicationClass();
            var doc = app.Documents.OpenNoRepairDialog(book.BookFileName, missing, missing, missing) as DocumentClass;
            var numOfWords = doc.Words.Count;
            var chapterCount = numOfWords / 10; // chapters

            var chapter = doc.Range((chapterCount * chapterNumber) - chapterCount, chapterCount * chapterNumber);
            var chapterContent = new List<string>();
            foreach (Range sentence in chapter.Sentences)
            {
                foreach (Range word in sentence.Words)
                {
                    if (word.Bold == -1)
                    {
                        chapterContent.Add(sentence.Text);
                        break;
                    }
                }
            }
            doc.Close();
            app.Quit();

            var bookContent = "";
            chapterContent.ForEach(c => bookContent += c);
            book.BookOpened = true;
            book.BookType = BookType.WORD;
            book.ChaptersRead += 1;
            book.LastRead = DateTime.Now;

            var chapterDB = new Chapter();
            chapterDB.ChapterOpened = true;
            chapterDB.Content = bookContent;
            chapterDB.LastRead = DateTime.Now;
            chapterDB.Number = chapterNumber;
            book.Chapters.Add(chapterDB);

            _bookReposistory.Update(book);
            await _bookReposistory.SaveAsync();
        }

        public async System.Threading.Tasks.Task OpenChapterAvailble(int bookId, int chapNumber)
        {
            var chapterExists = _bookReposistory.Where(c => c.Id == bookId && c.Chapters.Where(ch => ch.Number == chapNumber).Any()).Any();
            var book = _bookReposistory.Where(c => c.Id == bookId)
                            .AsQueryable().Include(b => b.Chapters).FirstOrDefault();
            if (chapterExists)
            {
                var chapId = book.Chapters.Where(c => c.Number == chapNumber).FirstOrDefault().Id;
                await OpenAChapter(chapId);
            }
            else
            {
                await OpenNewChapter(book, chapNumber);
                var chapId = book.Chapters.Where(c => c.Number == chapNumber).FirstOrDefault().Id;
                await OpenAChapter(chapId);
            }
        }
    }
}