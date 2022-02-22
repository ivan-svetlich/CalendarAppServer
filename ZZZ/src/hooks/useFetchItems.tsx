import { DateTime } from "luxon";
import { useEffect, useReducer, useState } from "react";
import { ItemDto } from "../entities/Item";
import { getItemsByInterval } from "../calendarServer";

export type ItemsState = {
    loading: boolean,
    data: Items[],
    error: string | null
}

type Items = {
    dueDate: Date,
    todoItems: ItemDto[]
}

type ACTION = { type: "LOAD", payload: null } | { type: "SUCCESS"; payload: Items[] } | { type: "FAILURE", payload: string};

const initialState: ItemsState = { loading: false, data: [], error: null };

function fetchItemsReducer<ItemsState>(state: ItemsState, action: ACTION) {
  const { type, payload } = action;

  switch (type) {
    case 'LOAD':
      return { ...state, loading: true, data: null, error: null };
    case 'SUCCESS':
      return { ...state, loading: false, data: payload, error: null };
    case 'FAILURE':
      return { ...state, loading: false, data: null, error: payload };
    default:
      return state;
  }
};

type Week = {
    start: DateTime,
    end: DateTime
}

export const useFetchItems = (weeks: Week[], index: number)
    : [ItemsState, (itemDto: ItemDto, index: number, replace?: boolean) => void] => {
    const [state, dispatch] = useReducer(fetchItemsReducer, initialState);

    useEffect(() => {
      if(weeks.length > index) {
        fetchItems(weeks[index].start, weeks[index].end, dispatch);
      }      
    }, [index, weeks])

    const updateState = (itemDto: ItemDto, index: number, replace: boolean = false) => {
      let updatedState = {...(state as ItemsState)};
      if(replace) {
        updatedState.data[index].todoItems = updatedState.data[index].todoItems.map((item => {
          if(item.id === itemDto.id) {
            return itemDto;
          }
          else {
            return item;
          }
        }))
      }
      else {
        updatedState.data[index].todoItems.push(itemDto);
      } 
      dispatch({ type: 'SUCCESS', payload: [...updatedState.data] });
    }

    return [state as ItemsState, updateState];
}

async function fetchItems(startDate: DateTime, endDate: DateTime, dispatch: React.Dispatch<ACTION>) {
    dispatch({ type: 'LOAD', payload: null });
    try {
        await getItemsByInterval(startDate, endDate)
            .then(response => {
                dispatch({ type: 'SUCCESS', payload: response.data });
            }); 
    }
    catch (error: any) {
        dispatch({ type: 'FAILURE', payload: error });
    }

}

export default useFetchItems;