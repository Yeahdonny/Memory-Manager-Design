using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memory_Policy_Simulator
{
    class Core
    {
        private int cursor;
        static public int cursor2 = 0;
        public int fcursor;
        public int p_frame_size;
        public List<Page> frame_window;
        public List<Page> pageHistory;
        public List<bool> reference = new List<bool>();
        public List<int> recountsave = new List<int>();
        public int hit;
        public int fault;
        public int migration;



        public Core(int get_frame_size)
        {
            this.fcursor = 0;
            cursor2 = 0;
            this.cursor = 0;
            this.p_frame_size = get_frame_size;
            this.frame_window = new List<Page>();
            this.pageHistory = new List<Page>();
        }

        public Page.STATUS Operate(char data)
        {
            Page newPage;

            if (this.frame_window.Any<Page>(x => x.data == data))
            {
                newPage.pid = Page.CREATE_ID++;
                newPage.data = data;
                newPage.status = Page.STATUS.HIT;                   //hit
                newPage.bit = 0;
                this.hit++;
                int i;

                for (i = 0; i < this.frame_window.Count; i++)
                {
                    if (this.frame_window.ElementAt(i).data == data) break;
                }
                newPage.loc = i + 1;

            }
            else
            {
                newPage.pid = Page.CREATE_ID++;
                newPage.data = data;

                if (frame_window.Count >= p_frame_size)
                {
                    newPage.status = Page.STATUS.MIGRATION;     //migration
                    if (cursor2 == p_frame_size)
                        cursor2 = 0;
                    this.frame_window.RemoveAt(cursor2);             //지우기도 뭘 지울지 내가 그뭐냐 설정해줘야됨.
                    fcursor = cursor2;
                    cursor2++;
                    cursor = cursor2;
                    this.migration++;
                    this.fault++;
                }
                else
                {
                    newPage.status = Page.STATUS.PAGEFAULT;         //page fault 값이 새로 들어올 때
                    cursor++;       //위친가?
                    this.fault++;

                }

                newPage.loc = cursor;
                newPage.bit = 0;
                if (this.migration != 0)
                    frame_window.Insert(fcursor, newPage);
                else
                    frame_window.Add(newPage);
            }
            pageHistory.Add(newPage);

            return newPage.status;
        }

        /********************LRU***************************************/
        public Page.STATUS LRU(char data)
        {
            Page newPage;
            int count;
            List<int> countsave = new List<int>();
            if (this.frame_window.Any<Page>(x => x.data == data))
            {
                newPage.pid = Page.CREATE_ID++;
                newPage.data = data;
                newPage.status = Page.STATUS.HIT;                   //hit
                newPage.bit = 0;
                this.hit++;
                int i;

                for (i = 0; i < this.frame_window.Count; i++)
                {
                    if (this.frame_window.ElementAt(i).data == data) break;
                }
                newPage.loc = i + 1;

            }
            else
            {
                newPage.pid = Page.CREATE_ID++;
                newPage.data = data;

                if (frame_window.Count >= p_frame_size)
                {
                    newPage.status = Page.STATUS.MIGRATION;     //migration

                    for (int i = 0; i < frame_window.Count; i++)
                    {
                        count = 0;
                        for (int j = pageHistory.Count - 1; j >= 0; j--)
                        {
                            if (this.pageHistory.ElementAt(j).data == this.frame_window.ElementAt(i).data)
                            {
                                countsave.Insert(i, count);
                                break;
                            }
                            else
                                count++;
                        }
                    }
                    int big = countsave.ElementAt(0);
                    int old = 0;
                    for (int i = 1; i < countsave.Count; i++)
                    {
                        if (big < countsave.ElementAt(i))
                        {
                            big = countsave.ElementAt(i);
                            old = i;
                        }
                    }
                    countsave.Clear();
                    cursor2 = old;
                    this.frame_window.RemoveAt(cursor2);             //지우기도 뭘 지울지 내가 그뭐냐 설정해줘야됨.
                    fcursor = cursor2;
                    cursor2++;
                    cursor = cursor2;
                    this.migration++;
                    this.fault++;

                }
                else
                {
                    newPage.status = Page.STATUS.PAGEFAULT;         //page fault 값이 새로 들어올 때
                    cursor++;       //위친가?
                    this.fault++;
                }

                newPage.loc = cursor;
                newPage.bit = 0;
                if (this.migration != 0)
                    frame_window.Insert(fcursor, newPage);
                else
                    frame_window.Add(newPage);
            }
            pageHistory.Add(newPage);

            return newPage.status;
        }
        /***********************OPTIMAL*****************************************/
        public Page.STATUS OPTIMAL(char data, List<char> datalist)
        {
            Page newPage;
            int count=0;

            List<int> countsave = new List<int>();
            if (this.frame_window.Any<Page>(x => x.data == data))
            {
                newPage.pid = Page.CREATE_ID++;
                newPage.data = data;
                newPage.status = Page.STATUS.HIT;                   //hit
                newPage.bit = 0;
                this.hit++;
                int i;

                for (i = 0; i < this.frame_window.Count; i++)
                {
                    if (this.frame_window.ElementAt(i).data == data) break;
                }
                newPage.loc = i + 1;

            }
            else
            {
                newPage.pid = Page.CREATE_ID++;
                newPage.data = data;

                if (frame_window.Count >= p_frame_size)
                {
                    newPage.status = Page.STATUS.MIGRATION;     //migration

                    for (int i = 0; i < frame_window.Count; i++)
                    {
                        count = 0;

                        for (int j = pageHistory.Count + 1; j < datalist.Count; j++)
                        {
                            if (this.frame_window.ElementAt(i).data.Equals(datalist.ElementAt(j)))
                            {
                                // countsave.Insert(i, count);
                                char c = frame_window.ElementAt(i).data;
                                break;
                            }
                            else
                            {
                                char d = datalist.ElementAt(j);
                                count++;
                            }
                        }
                        countsave.Insert(i, count);

                    }
                    int big = countsave.ElementAt(0);
                    int old = 0;
                    for (int i = 1; i < countsave.Count; i++)
                    {
                        if (big < countsave.ElementAt(i))
                        {
                            big = countsave.ElementAt(i);
                            old = i;
                        }
                    }
                    countsave.Clear();
                    cursor2 = old;
                    this.frame_window.RemoveAt(cursor2);             //지우기도 뭘 지울지 내가 그뭐냐 설정해줘야됨.
                    fcursor = cursor2;
                    cursor2++;
                    cursor = cursor2;
                    this.migration++;
                    this.fault++;

                }
                else
                {
                    newPage.status = Page.STATUS.PAGEFAULT;         //page fault 값이 새로 들어올 때
                    cursor++;       //위친가?
                    this.fault++;
                }

                newPage.loc = cursor;
                newPage.bit = 0;
                if (this.migration != 0)
                    frame_window.Insert(fcursor, newPage);
                else
                    frame_window.Add(newPage);
            }

            pageHistory.Add(newPage);

            return newPage.status;
        }

        /****************second*************************************/
        public Page.STATUS SECOND(char data)
        {
            Page newPage;


            List<int> countsave = new List<int>();
            if (this.frame_window.Any<Page>(x => x.data == data))
            {
                newPage.pid = Page.CREATE_ID++;
                newPage.data = data;
                newPage.status = Page.STATUS.HIT;                   //hit

                this.hit++;
                int i;

                for (i = 0; i < this.frame_window.Count; i++)
                {
                    if (this.frame_window.ElementAt(i).data == data) break;
                }
                newPage.loc = i + 1;
                /*   if (reference.ElementAt(i))
                   {
                       newPage.bit = 0;
                       reference.Insert(i, false);
                   }
                   else
                   {*/
                newPage.bit = 1;
                reference[i] = true;
                //}
            }
            else
            {
                newPage.pid = Page.CREATE_ID++;
                newPage.data = data;

                if (frame_window.Count >= p_frame_size)
                {
                    newPage.status = Page.STATUS.MIGRATION;     //migration

                    for (int i = 0; i < reference.Count; i++)
                    {
                        if (reference.ElementAt(cursor2))
                        {

                            reference[cursor2] = false;
                            cursor2++;
                            if (cursor2 == reference.Count)
                                cursor2 = 0;
                        }
                        else
                            break;
                    }
                    this.frame_window.RemoveAt(cursor2);             //지우기도 뭘 지울지 내가 그뭐냐 설정해줘야됨.
                    this.reference.RemoveAt(cursor2);
                    fcursor = cursor2;
                    cursor2++;
                    cursor = cursor2;
                    if (cursor2 == p_frame_size)
                        cursor2 = 0;
                    this.migration++;
                    this.fault++;


                }
                else
                {
                    newPage.status = Page.STATUS.PAGEFAULT;         //page fault 값이 새로 들어올 때
                    reference.Add(false);
                    cursor++;       //위친가?
                    this.fault++;
                }

                newPage.loc = cursor;
                newPage.bit = 0;
                if (this.migration != 0)
                {
                    frame_window.Insert(fcursor, newPage);
                    reference.Insert(fcursor, false);
                }
                else
                    frame_window.Add(newPage);

            }

            pageHistory.Add(newPage);

            return newPage.status;
        }
        /******************LFU**************************************/
        public Page.STATUS LFU(char data)
        {
            Page newPage;


            if (this.frame_window.Any<Page>(x => x.data == data))
            {
                newPage.pid = Page.CREATE_ID++;
                newPage.data = data;
                newPage.status = Page.STATUS.HIT;                   //hit
                newPage.bit = 0;
                this.hit++;
                int i;

                for (i = 0; i < this.frame_window.Count; i++)
                {
                    if (this.frame_window.ElementAt(i).data == data) break;
                }
                newPage.loc = i + 1;

                recountsave[i] = recountsave[i] + 1;
            }
            else
            {
                newPage.pid = Page.CREATE_ID++;
                newPage.data = data;

                if (frame_window.Count >= p_frame_size)
                {
                    newPage.status = Page.STATUS.MIGRATION;     //migration

                    int big = recountsave.ElementAt(0);
                    int old = 0;
                    for (int i = 1; i < recountsave.Count; i++)
                    {
                        if (big > recountsave.ElementAt(i))
                        {
                            big = recountsave.ElementAt(i);
                            old = i;
                        }
                    }

                    cursor2 = old;
                    this.frame_window.RemoveAt(cursor2);             //지우기도 뭘 지울지 내가 그뭐냐 설정해줘야됨.
                    recountsave.RemoveAt(cursor2);
                    fcursor = cursor2;
                    cursor2++;
                    cursor = cursor2;
                    this.migration++;
                    this.fault++;

                }
                else
                {
                    newPage.status = Page.STATUS.PAGEFAULT;         //page fault 값이 새로 들어올 때
                    cursor++;       //위친가?
                    this.fault++;
                }

                newPage.loc = cursor;
                newPage.bit = 0;
                if (this.migration != 0)
                {
                    frame_window.Insert(fcursor, newPage);
                    recountsave.Insert(fcursor, 0);
                }
                else
                {
                    frame_window.Add(newPage);
                    recountsave.Add(0);
                }
            }
            pageHistory.Add(newPage);

            return newPage.status;
        }
        /****************MFU************************************/
        public Page.STATUS MFU(char data)
        {
            Page newPage;
            int count = 0;

            if (this.frame_window.Any<Page>(x => x.data == data))
            {
                newPage.pid = Page.CREATE_ID++;
                newPage.data = data;
                newPage.status = Page.STATUS.HIT;                   //hit
                newPage.bit = 0;
                this.hit++;
                int i;

                for (i = 0; i < this.frame_window.Count; i++)
                {
                    if (this.frame_window.ElementAt(i).data == data) break;
                }
                newPage.loc = i + 1;

                recountsave[i] = recountsave[i] + 1;
            }
            else
            {
                newPage.pid = Page.CREATE_ID++;
                newPage.data = data;

                if (frame_window.Count >= p_frame_size)
                {
                    newPage.status = Page.STATUS.MIGRATION;     //migration

                    int big = recountsave.ElementAt(0);
                    int old = 0;
                    for (int i = 1; i < recountsave.Count; i++)
                    {
                        if (big < recountsave.ElementAt(i))
                        {
                            big = recountsave.ElementAt(i);
                            old = i;
                        }
                    }

                    cursor2 = old;
                    this.frame_window.RemoveAt(cursor2);             //지우기도 뭘 지울지 내가 그뭐냐 설정해줘야됨.
                    recountsave.RemoveAt(cursor2);
                    fcursor = cursor2;
                    cursor2++;
                    cursor = cursor2;
                    this.migration++;
                    this.fault++;

                }
                else
                {
                    newPage.status = Page.STATUS.PAGEFAULT;         //page fault 값이 새로 들어올 때
                    cursor++;       //위친가?
                    this.fault++;
                }

                newPage.loc = cursor;
                newPage.bit = 0;
                if (this.migration != 0)
                {
                    frame_window.Insert(fcursor, newPage);
                    recountsave.Insert(fcursor, 0);
                }
                else
                {
                    frame_window.Add(newPage);
                    recountsave.Add(0);
                }
            }
            pageHistory.Add(newPage);

            return newPage.status;
        }











        public List<Page> GetPageInfo(Page.STATUS status)
        {
            List<Page> pages = new List<Page>();

            foreach (Page page in pageHistory)
            {
                if (page.status == status)
                {
                    pages.Add(page);
                }
            }

            return pages;
        }

    }


}
