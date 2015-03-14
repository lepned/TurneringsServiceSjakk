namespace global

open System

type SpillerInfo = {Gruppe:string; Navn:string; Rating: int option ; mutable IsChecked: Nullable<bool>}

type Player = {Id:int;Name:string;Rating:int option;StartNr:int;Points:float;Gruppe:string;ColorScore:int}

//type Player(name:string,id:int) =
//        member this.Id = id
//        member this.Name=name
//        member val StartNr = id with get,set
//        member val GamesPlayed = List.empty<Board> with get,set
//        member val Points = 0.0 with get,set
//        member val ColorScore = 0 with get,set //white - black gives a score
//        member val SortedStartNr = 0.0 with get,set
//        
//        new (id:int) = new Player("Player " + id.ToString() , id )
//        override this.ToString() = sprintf "%s" name

and Board(id:int, WhitePlayer:Player, BlackPlayer:Player, result:Result) =
    member this.Id=id
    member val WhitePlayer= WhitePlayer with get,set
    member val BlackPlayer= BlackPlayer with get,set
    member val Result = result with get,set
    member val Board = Unchecked.defaultof<System.Uri> with get,set
    member this.HvitNede = if id%2=1 then true else false
    new (id:int)= Board(id, Unchecked.defaultof<Player>, Unchecked.defaultof<Player>, Result.None)
    override this.ToString() = sprintf "%i: %A" id result

and Result=
    |WhiteWon = 0
    |BlackWon = 1
    |Draw = 2
    |None = 3

type Runde = {Id:int; Boards:Board seq }

type Berger = {Dato:System.DateTime; Runder: Runde list; Spillere: Player list}


