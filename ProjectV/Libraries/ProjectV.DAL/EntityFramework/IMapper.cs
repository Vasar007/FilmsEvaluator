﻿namespace ProjectV.DAL.EntityFramework
{
    internal interface IMapper<TFirst, TSecond>
    {
        TFirst Map(TSecond model);

        TSecond Map(TFirst model);
    }
}
