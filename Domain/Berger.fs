namespace global

open System
open System.IO
open DataLoader

module BergerOperations =      
 
    let swap (a,b) = (b,a)
    let rng = System.Random()
 
    let setColor runde bord pair =
        match bord % 2 = 1 with
        |true when bord=1 -> if runde % 2 = 1 then pair else swap pair
        |true -> pair
        |_ -> swap pair
 
    let initPlayers = 
        let init = getSpillere
        if (init|>List.length) % 2 = 1 then 
            let wo =  {Id = 1000;Name="Wo";Rating=None;Gruppe="A";StartNr=1000;Points=0.0;ColorScore=0}
            init@[wo] else init
 
    let rotateBerger list =
        let arr = List.toArray list
        let lengde = arr.Length-1
        [   yield arr.[lengde - 1]
            yield! arr.[0 ..lengde - 2]
            yield arr.[lengde]]
 
    let getPairs list =
        let lengde = list|>List.length
        [for i=0 to list.Length/2-1 do yield list.[i],list.[lengde-i-1] ]
 
    let getRundeOppsett runde list =    
        let rec loop n lst =
            match n with
            |1 -> lst
            |_ -> loop (n-1) (rotateBerger lst)    
        let boards=
            getPairs (loop runde list)
            |>List.mapi (fun idx pair -> idx+1,pair)
            |>List.map (fun (bordId,pair) -> 
                let w,b=setColor runde bordId pair
                Board(bordId,w,b,Result.None))
        {Id=runde;Boards=boards}
 
    let bergerRunder n =
        let players = initPlayers
        let antrunder = players.Length-1
        let runder = [for i in 1 .. antrunder -> getRundeOppsett i players]
        let dato=System.DateTime.Now
        {Dato=dato;Runder=runder; Spillere=players}
 
    let countColors (berger:Berger) =
        let coll = berger.Runder |> Seq.collect (fun runde -> runde.Boards)
        let whites = coll |>Seq.countBy (fun bord -> bord.WhitePlayer)|> Seq.sortBy(fun (player,ant) -> ant) |> Seq.toList
        let blacks = coll |>Seq.countBy (fun bord -> bord.BlackPlayer) |>Seq.sortBy(fun (player,ant) -> ant) |> Seq.toList
        List.zip whites blacks |> List.map (fun (w,b) -> sprintf "%A: Hvit %i Sort %i" (fst w) (snd w) (snd b))
 
    let berger = bergerRunder 4
 
    let updateResult runde =
        let result = [Result.WhiteWon;Result.BlackWon;Result.Draw]
        let bordene =
            [for bord in runde.Boards do
                let dice = result.[rng.Next(result.Length)]
                match bord.WhitePlayer,bord.BlackPlayer with
                |WO,_ -> bord.Result <- Result.BlackWon
                |_,WO -> bord.Result <- Result.WhiteWon
                |_ ->    bord.Result <- dice]        
        {runde with Boards=bordene}
 
    let bergerWithResults (berger:Berger) = 
        let rec loop list =
            match list with
            |[] -> []
            |h::t ->
                updateResult h :: loop t
        loop berger.Runder
 
    let result = bergerWithResults berger

    let draw (players:Player seq) = 
        players                
        |> Seq.map (fun p -> if p.Id>500 then 0.9999999,p else rng.NextDouble(), p)
        |> Seq.sortBy fst
        |> Seq.map snd
        |> Seq.mapi( fun idx p -> {p with StartNr=idx+1})
        
