; (function (window) {
    var Hierarchical = function (root, childSelector) {
        var hierarchical = {};
        hierarchical.current = root;
        hierarchical._childSelector = childSelector;
        hierarchical.parent = null;
        hierarchical.children = initializeChildren(hierarchical);
        hierarchical.root = hierarchical;

        return hierarchical;
    };

    function pHierarchical(node, parent, childSelector) {
        var hierarchical = {};
        hierarchical.current = node;
        hierarchical._childSelector = childSelector;
        hierarchical.parent = parent;
        hierarchical.children = initializeChildren(hierarchical);
        hierarchical.root = (function (node) {
            var re = node;
            while (re.parent !== null) {
                re = re.parent;
            }
            return re;
        })(hierarchical);

        return hierarchical;
    }

    function initializeChildren (hierarchical) {
        var selected = hierarchical._childSelector(hierarchical.current);
        var children = [];

        for (var i = 0; i < selected.length; i++) {
            children.push(pHierarchical(selected[i], hierarchical, hierarchical._childSelector));
        }

        return children;
    }

    window.Hierarchical = Hierarchical;
})(window);