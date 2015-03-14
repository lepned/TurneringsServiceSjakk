namespace global

open System.Collections.ObjectModel

module Conversion =
    open System.Windows
    open System.Windows.Media
    open System.Diagnostics
    open System.Windows.Controls

    let toObservable list =
        let obs = new ObservableCollection<SpillerInfo>()
        list|>Seq.iter(fun el -> obs.Add el)
        obs

    let ShowVisualTree(obj: DependencyObject) =
        let rec browse indentlvl node =
            let indent n = for i = 0 to (n-1) do printf " "
            let n = VisualTreeHelper.GetChildrenCount(node)
            indent indentlvl
            let nodename = sprintf "<%s%s>" (node.GetType().Name) (if n=0 then "/" else "")
            Debug.WriteLine <| nodename
            for i =0 to (n-1) do
                browse (indentlvl + 1) (VisualTreeHelper.GetChild(node,i))
            if n>0 then 
                indent indentlvl
                printfn "</%s>" (node.GetType().Name)

        browse 0 obj


    let GetElement (obj: DependencyObject) =
        let res = ref null
        
        let rec browse indentlvl node =
            let n = VisualTreeHelper.GetChildrenCount(node)
            for i =0 to (n-1) do
                let child = VisualTreeHelper.GetChild(node,i)
                match child with
                | :? CheckBox as chbox -> if res.Value = null then res := chbox 
                |_ -> browse (indentlvl + 1) (VisualTreeHelper.GetChild(node,i))
        browse 0 obj
        
        res.Value
