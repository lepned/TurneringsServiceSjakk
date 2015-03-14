namespace global

open System
open Microsoft.FSharp.Linq
open Linq.NullableOperators
open System.Collections.ObjectModel
open System.Windows.Controls
open System.Diagnostics
open System.Windows
open DataLoader
open System.Windows.Media
open System.Windows.Input
open Conversion
open UIElements

//A symbology Symbology callback is example of external dependency. Can be interface or abstract type as well.
type MainContoller() =  

    interface IController<MainEvents, MainModel> with

        member this.InitModel model = 
            model.AddedPlayer <- {Navn=""; Rating = Some 0; Gruppe="B"; IsChecked= Nullable true}
            let spillere= getSpillere
            model.AllPlayers <- Conversion.toObservable (spillere|>List.map (fun sp -> {Navn=sp.Name; Rating=sp.Rating;Gruppe=sp.Gruppe; IsChecked= Nullable true})) 
            model.NavneListe <- model.AllPlayers //Conversion.toObservable (spillere|>List.map (fun sp -> {Navn=sp.Name; Rating=sp.Rating;Gruppe=sp.Gruppe; IsChecked= Nullable true}))            
            
        member this.EventHandler(model, event) = 
            match event with
            | BergerTabell -> this.CreateBergerTabell(model)
            | AddToList -> this.AddToList(model)
            | SelectionChanged arg -> this.SelectedPlayer(model,arg)
            | TextChanged arg -> this.TextChangedEvent (model,arg)
            | RemovePlayer arg -> this.RemovePlayer (model,arg) 
            | SortByCheckBox -> this.Sorter(model)
            | AllPlayersInList -> this.AllPlayers (model)

    member this.RemovePlayer (model:MainModel, arg: Input.MouseEventArgs) =
//        if arg.RightButton = MouseButtonState.Pressed then Debug.WriteLine "RightButton pressed" else Debug.WriteLine "Waiting"        
        ()
    
    member this.AllPlayers(model: MainModel) =
        model.NavneListe <- model.AllPlayers|>Seq.map(fun e -> e)|>toObservable

    member this.CreateBergerTabell(model: MainModel) =         
        ()

    member this.Sorter (model: MainModel) =            
        model.NavneListe <- 
            model.AllPlayers
            |>Seq.filter(fun e -> e.IsChecked.Value)
            |>toObservable 
//        let main = MainWindow()
//        main.Show()
        let mainWindow = StartWindow()
        let mvc = Model.Create(), MainView mainWindow, MainContoller()
        use eventLoop = Mvc.start mvc
        mainWindow.Show()
    
    member this.AddToList(model:MainModel) =               
        if model.AddedPlayer.Navn="" then ()
        else model.NavneListe.Add(model.AddedPlayer)

    member this.SelectedPlayer(model:MainModel, arg:SelectionChangedEventArgs) =        
        match arg.Source with
        | :? ListBox as listbox ->  
            model.SelectedPlayer <- listbox.SelectedItems.[0] :?> SpillerInfo
        |_ -> ()

    member this.TextChangedEvent(model:MainModel, arg:RoutedEventArgs) =
        match arg.Source with
        | :? TextBox as tbox ->
            match tbox.Name with
            |"Navn" -> 
                model.AddedPlayer <- {model.AddedPlayer with Navn=tbox.Text}
                arg.Handled <- true
            |"Rating" -> 
                let ok, rating = Int32.TryParse(tbox.Text)
                if ok then model.AddedPlayer <- {model.AddedPlayer with Rating=Some rating}
                else ()
            |_ -> ()
            
        |_ -> ()
        
