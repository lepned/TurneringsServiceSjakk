namespace global
open System
module DataLoader =

    open System.IO
    let path = """C:\Users\lepne_000\documents\visual studio 2013\Projects\TurneringsServiceSSK\Operations\Deltagere.txt"""

    let textContent = File.ReadAllLines(path)

    //type Spiller = {Gruppe:string; Navn:string; Rating: int option; Motstandere: Spiller list; Poeng:float}
    
    let initPlayer idx info = {Id=idx;Name=info.Navn;Rating=info.Rating;Gruppe=info.Gruppe;StartNr=0;Points=0.0;ColorScore=0}

    let splitText (text:string) =
        let delt = text.Split(',')
        let rating = 
            match System.Int32.TryParse(delt.[2]) with
            |true, r -> Some r
            |false,_ -> None 
        {Gruppe=delt.[0];Navn=delt.[1]; Rating=rating; IsChecked= Nullable false}

    let spillerListe (text:string []) =
        let header = text.[0]
        let resten = text.[1..]
        [for sp in resten do
            yield splitText (sp.Trim())]

    let getSpillere  = spillerListe textContent  |>List.mapi(fun idx info -> initPlayer idx info)

    let a,b,c = //sortert etter grupper
        getSpillere|>List.filter(fun sp -> sp.Gruppe="A" ),
        getSpillere|>List.filter(fun sp -> sp.Gruppe="B" ),
        getSpillere|>List.filter(fun sp -> sp.Gruppe="C" )

//    let initSpiller (info:SpillerInfo list) =
//        info
//        |>List.map (fun info -> {Gruppe=info.Gruppe; Navn=info.Navn; Rating=info.Rating; Motstandere=[]; Poeng=0.0})
//
//    let spillere = initSpiller getSpillere |> List.sortBy (fun sp -> sp.Rating)
//
//    let updateSpiller (spiller:Spiller) (motstander:Spiller) =
//        {spiller with Motstandere=spiller.Motstandere @ [motstander]}
//
//    let updatePar (hvit,sort) =
//        updateSpiller hvit sort,
//        updateSpiller sort hvit
//
//    let rec getPair (spillere:Spiller list) =
//        match spillere with
//        |h::s::rest -> (h,s)::getPair rest
//        |_ -> []
//
//    let t1 = updatePar (spillere.[0],spillere.[2])
//
//    let pairTest = getPair spillere
//
//    let pairToList (a,b) = a::[b]
//
//    let updateRunde (runde:(Spiller*Spiller) list) =
//        runde 
//        |> List.map updatePar
//        |> List.map pairToList
//        |> List.concat
//
//    let rundeTest = updateRunde pairTest

