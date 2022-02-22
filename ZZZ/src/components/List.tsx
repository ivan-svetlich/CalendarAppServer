import "./listStyles.scss";
import { DateTime } from "luxon";
import { useEffect, useRef, useState } from "react";
import useCalendar from "../hooks/useCalendar";
import useFetchItems from "../hooks/useFetchItems";
import { addItem, updateItem } from "../calendarServer";
import { ItemDto } from "../entities/Item";

const List = () => {
    const [selectedYear, setSelectedYear] = useState(DateTime.now().year)
    const [selectedMonth, setSelectedMonth] = useState(DateTime.now().month as number);
    const weeks = useCalendar(selectedMonth, selectedYear);
    const [selectedWeek, setSelectedWeek] = useState<number>(currentWeek);
    const [lastWeek, setLastWeek] = useState(false)
    

    
    const [itemsState, updateState] = useFetchItems(weeks, selectedWeek)
    const handleSelectMonth = (e: React.ChangeEvent<HTMLSelectElement>) => {
        setLastWeek(false);
        setSelectedMonth(Number.parseInt(e.target.value));
        setSelectedWeek(0);
    }

    function currentWeek() {
        const index = weeks.findIndex((interval) => (
            interval.start < DateTime.now() && interval.end > DateTime.now()
        ) )
        if(index >= 0) {
            return index;
        }
        else { return 0 }
    }

    const handlePreviousWeek = () => {
        if(selectedWeek > 0) {
            setLastWeek(false);
            setSelectedWeek(prev => prev - 1);
        }
        else if(selectedMonth > 1){
            setSelectedMonth(prev => prev - 1);
            setLastWeek(true);
        }
        else {
            setSelectedYear(prev => prev - 1);
            setSelectedMonth(12);
            setLastWeek(true);
        }
        
    }

    useEffect(() => {
        if(lastWeek) {
            setSelectedWeek(weeks.length - 1);
        }
    }, [lastWeek, weeks.length])

    const handleNextWeek = () => {
        if(selectedWeek < weeks.length - 1) {
            setLastWeek(false);
            setSelectedWeek(prev => prev + 1);
        }
        else if(selectedMonth < 12){
            setLastWeek(false);
            setSelectedMonth(prev => prev + 1);
            setSelectedWeek(0);
        }
        else {
            setLastWeek(false);
            setSelectedYear(prev => prev + 1);
            setSelectedMonth(1);
            setSelectedWeek(0);
        }
        
    }

    const handlePreviousYear = () => {
        if(selectedYear > 0) {
            setLastWeek(false);
            setSelectedYear(prev => prev - 1);
        }
    }
    const handleNextYear = () => {
        setLastWeek(false);
        setSelectedYear(prev => prev + 1);
        setSelectedMonth(prev => prev);
    }
    const months = ["January", "February", "March", "April", "May", "June", 
        "July", "August", "September", "October", "November", "December"];


    const [inputs, setInputs] = useState<string[]>(["", "", "", "", "", "", ""])
    const handleAddItem = (e: React.MouseEvent<HTMLButtonElement, MouseEvent>, dueDate: Date) => {
        const weekday = Number.parseInt(e.currentTarget.id);
        const description = inputs[weekday];
        if(description) {
            addItem({ description, completed: false, removed: false, dueDate }).then((response) => {
                updateState(response.data, weekday);
                setInputs(prev => prev.map(
                                (content, index) => {
                                    if(index === weekday) {
                                        return "";
                                    }
                                    else {
                                        return content;
                                    }
                                }))
            });
        }
        
    }
    const createRemainingLines = (usedLines:number, weekday: number, dueDate: Date) => {
    const elements = [];

    elements.push(<div className="todo-line" key={`btn-${weekday}`}>
                        <span className="todo-description">
                            <button name="button" id={`${weekday}`}
                            type="button" className="add-btn btn" onClick={(e) => handleAddItem(e, dueDate)}>+</button>
                            <input className="add-input" value={inputs[weekday]}
                                onChange={(e) => setInputs(prev => prev.map(
                                (content, index) => {
                                    if(index === weekday) {
                                        return e.target.value;
                                    }
                                    else {
                                        return content;
                                    }
                                }))}/>
                        </span>
                    </div>  )

    if(usedLines < 3) {
        for (var i=0; i < 3 - usedLines; i++) {
            elements.push(<div className="todo-line" key={`${weekday}-${3-i}`}>
                        <span className="todo-description">
                        </span>
                        <span className="todo-completed">
                        </span>
                    </div>)
        } 
    }
    
    return elements;
  }

    const handleMarkCompleted = (item: ItemDto, index: number) => {
        const id = item.id;
        updateItem(id, {description: item.description, completed: true, removed: item.removed, dueDate: item.dueDate})
            .then(response => updateState(response.data, index, true));
        
    }
    return (
        <div id="list-container">
            <div className="list-date">
                <div className="month">
                    <select value={selectedMonth} onChange={e => handleSelectMonth(e)}>
                        {months.map((month, index) => (
                            <option value={index + 1} key={index + 1}>{month}</option>
                        ))}
                    </select>     
                </div>
                <div className="week">
                    <button className="add-btn">
                        <i className="fas fa-chevron-left" onClick={e => handlePreviousWeek()}></i>
                    </button>
                    <div className="current-week">
                        <span className="week-start">
                            {weeks.length > selectedWeek && `${weeks[selectedWeek].start.weekdayLong} ${weeks[selectedWeek].start.day}`}
                        </span>   
                        <span className="week-divider">
                            {` - `}
                        </span>  
                        <span className="week-end">
                            {weeks.length > selectedWeek && `${weeks[selectedWeek].end.weekdayLong} ${weeks[selectedWeek].end.day}`}
                        </span>  
                    </div>
                    <button className="add-btn" onClick={e => handleNextWeek()}>    
                        <i className="fas fa-chevron-right"></i>
                    </button>
                </div>
                <div className="year">
                    <button className="add-btn">
                        <i className="fas fa-chevron-left" onClick={e => handlePreviousYear()}></i>
                    </button>
                    <div className="current-week">
                        <span className="week-start">
                            {selectedYear}
                        </span>    
                    </div>
                    <button className="add-btn" onClick={e => handleNextYear()}>    
                        <i className="fas fa-chevron-right"></i>
                    </button>
                </div>
            </div>
            {itemsState.loading && 'Loading...'}
            {itemsState.data && itemsState.data.map((day, dayIndex) => (
                <div className="day-container" key={dayIndex}>
                <div className="day">
                    {`${DateTime.fromISO(day.dueDate.toString(), {zone: 'utc'}).weekdayLong} ${DateTime.fromISO(day.dueDate.toString(), {zone: 'utc'}).day}`}
                </div>
                <div className="daily-entries">
                    {day.todoItems && day.todoItems.map((item, index) => (
                        <div className="todo-line" key={`${index}-${dayIndex}`}>
                            <span className="todo-completed">
                                {item.completed ? '✓' : <button id={`${item.id}`} 
                                    className="done-btn" onClick={(e) => handleMarkCompleted(item, dayIndex)}>✓</button>}
                            </span>
                            <span className="todo-description">
                                {item.description}
                            </span>
                            
                        </div>
                    ))}
                    {day.todoItems && createRemainingLines(day.todoItems.length, dayIndex, day.dueDate)}                    
                </div>
            </div>
            )               
            )}
            {itemsState.data &&
            <div className="page-footer">
                <button className="add-btn" onClick={e => handlePreviousWeek()}>
                    <i className="fas fa-chevron-left"></i>
                </button>
                <div>
                    <span className="week-start">
                        {weeks.length > selectedWeek && `${weeks[selectedWeek].start.weekdayLong} ${weeks[selectedWeek].start.day}`}
                    </span>   
                    <span className="week-divider">
                        {` - `}
                    </span>  
                    <span className="week-end">
                        {weeks.length > selectedWeek && `${weeks[selectedWeek].end.weekdayLong} ${weeks[selectedWeek].end.day}`}
                    </span>  
                </div> 
                <button className="add-btn" onClick={e => handleNextWeek()}>
                    <i className="fas fa-chevron-right"></i>
                </button>
            </div>
            }
        </div>
    )
}

export default List;