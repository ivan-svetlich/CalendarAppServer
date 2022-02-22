import axios from "axios";
import { DateTime } from "luxon";
import { ItemDto } from "./entities/Item";

const TOKEN = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJJZCI6ImM0OThkNzA5LWZhMTAtNGRiMy04YmZjLWQxNDNkZWI2YzZlZSIsImVtYWlsIjoidXNlcjFAdXNlci5jb20iLCJzdWIiOiJjNDk4ZDcwOS1mYTEwLTRkYjMtOGJmYy1kMTQzZGViNmM2ZWUiLCJqdGkiOiJhZjhjY2NiZS1jNDUwLTQ1ZDUtYjI0Ni01YWVmMWJhZmIyMmEiLCJuYmYiOjE2NDU1MzQzODcsImV4cCI6MTY0NjEzOTE4NywiaWF0IjoxNjQ1NTM0Mzg3fQ.74Df4_1wY7k90RCURroX6kJDOasrSXeKD1Bvr7ZvZbM";

type Items = {
    dueDate: Date,
    todoItems: ItemDto[]
}

const getItemsByInterval = (startDate: DateTime, endDate: DateTime) => {
    return axios.get<Items[]>(
        `https://localhost:5001/Items?startDate=${startDate.toUTC().toISO()}&endDate=${endDate.toUTC().toISO()}`, 
            {headers: {"authorization": TOKEN}})
}

type UpdateItemRequest = {
    description: string,
    completed: boolean,
    removed: boolean,
    dueDate: Date
}
const addItem = (item: UpdateItemRequest) => {
    return axios.post(
        `https://localhost:5001/Items`, item,
            {headers: {"authorization": TOKEN}})
}

const updateItem = (id: number, item: UpdateItemRequest) => {
    return axios.put(
        `https://localhost:5001/Items/${id}`, item,
            {headers: {"authorization": TOKEN}})
}

export {
    addItem,
    getItemsByInterval,
    updateItem
};