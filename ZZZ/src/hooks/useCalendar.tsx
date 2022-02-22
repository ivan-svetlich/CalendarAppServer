import { useEffect, useState } from "react";
import { DateTime } from "luxon";

const useCalendar = (month: number, year: number) => {
    const getWeeksOfMonth = (_month: number = DateTime.now().month as number, 
        _year: number = DateTime.now().year) => { 
        let start = DateTime.utc(_year, _month).startOf('month').startOf('week');
        let end = DateTime.utc(_year, _month).startOf('month').endOf('week');
        const weeksOfMonth = [];

        while(end.month === _month) {
            weeksOfMonth.push(
                {start: start,
                end: end})
            start = start.plus({days: 7})
            end = end.plus({days: 7})
        }
        
        return weeksOfMonth;
    }

    type Week = {
        start: DateTime,
        end: DateTime
    }
    const [weeks, setWeeks] = useState<Week[]>(getWeeksOfMonth);

    useEffect(() => {
        const weeksOfMonth = getWeeksOfMonth(month, year);
        setWeeks([...weeksOfMonth]);
    }, [month, year])

    return weeks
}

export default useCalendar;