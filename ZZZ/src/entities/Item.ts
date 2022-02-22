import { DateTime } from "luxon";

export interface ItemDto {
    id: number,
    userId: string,
    description: string,
    completed: boolean,
    removed: boolean,
    dueDate: Date,
    createdOn: Date,
    updatedOn: Date
}

class Item {
    id;
    userId;
    description;
    completed;
    removed;
    dueDate;
    createdOn;
    updatedOn;

    constructor(
        {id,
        userId,
        description,
        completed,
        removed,
        dueDate,
        createdOn,
        updatedOn}: ItemDto
    ) {
        this.id = id;
        this.userId = userId;
        this.description = description;
        this.completed = completed;
        this.removed = removed;
        this.dueDate = DateTime.fromJSDate(dueDate);
        this.createdOn = DateTime.fromJSDate(createdOn);
        this.updatedOn = DateTime.fromJSDate(updatedOn);
    }
}

export default Item;
